using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Core.WindJson;

namespace Pomelo.Protobuf
{
    public class MsgDecoder
    {
        private JsonNode protos { set; get; }//The message format(like .proto file)
        private int offset { set; get; }
        private byte[] buffer { set; get; }//The binary message from server.
        private Util util { set; get; }

        public MsgDecoder(JsonNode protos)
        {
            if (protos == null) protos = new JsonClass();

            this.protos = protos;
            this.util = new Util();
        }

        /// <summary>
        /// Decode message from server.
        /// </summary>
        /// <param name='route'>
        /// Route.
        /// </param>
        /// <param name='buf'>
        /// JsonObject.
        /// </param>
        public JsonNode decode(string route, byte[] buf)
        {
            this.buffer = buf;
            this.offset = 0;
            JsonNode proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                JsonNode msg = new JsonClass();
                return this.decodeMsg(msg, proto, this.buffer.Length);
            }
            return null;
        }


        /// <summary>
        /// Decode the message.
        /// </summary>
        /// <returns>
        /// The message.
        /// </returns>
        /// <param name='msg'>
        /// JsonObject.
        /// </param>
        /// <param name='proto'>
        /// JsonObject.
        /// </param>
        /// <param name='length'>
        /// int.
        /// </param>
        private JsonNode decodeMsg(JsonNode msg, JsonNode proto, int length)
        {
            while (this.offset < length)
            {
                Dictionary<string, int> head = this.getHead();
                int tag;
                if (head.TryGetValue("tag", out tag))
                {
                    JsonNode _tags = null;
                    if (proto.TryGetValue("__tags", out _tags))
                    {
                        JsonNode name = null;
                        if (_tags.TryGetValue(tag.ToString(), out name))
                        {
                            JsonNode value = null;
                            if (proto.TryGetValue(name.AsString, out value))
                            {
                                JsonNode option;
                                if (value.TryGetValue("option", out option))
                                {
                                    switch (option.ToString())
                                    {
                                        case "optional":
                                        case "required":
                                            JsonNode type = null;
                                            if (value.TryGetValue("type", out type))
                                            {
                                                msg.Add(name.AsString, this.decodeProp(type.AsString, proto));
                                            }
                                            break;
                                        case "repeated":
                                            JsonNode _name = null;
                                            if (!msg.TryGetValue(name.AsString, out _name))
                                            {
                                                msg.Add(name.AsString, new JsonArray());
                                            }
                                            JsonNode value_type = null;
                                            if (msg.TryGetValue(name.AsString, out _name) && (value.TryGetValue("type", out value_type)))
                                            {
                                                decodeArray(_name as JsonArray, value_type.AsString, proto);
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return msg;
        }

        /// <summary>
        /// Decode array in message.
        /// </summary>
        private void decodeArray(JsonArray list, string type, JsonNode proto)
        {
            if (this.util.isSimpleType(type))
            {
                int length = (int)Decoder.decodeUInt32(this.getBytes());
                for (int i = 0; i < length; i++)
                {
                    list.Add(this.decodeProp(type, null));
                }
            }
            else
            {
                list.Add(this.decodeProp(type, proto));
            }
        }

        /// <summary>
        /// Decode each simple type in message.
        /// </summary>
        private JsonNode decodeProp(string type, JsonNode proto)
        {
            switch (type)
            {
                case "uInt32":
                    // TODO: Ç±ÔÚµÄ´íÎó
                    return new JsonData((int)Decoder.decodeUInt32(this.getBytes()));
                case "int32":
                case "sInt32":
                    return new JsonData(Decoder.decodeSInt32(this.getBytes()));
                case "float":
                    return new JsonData(this.decodeFloat());
                case "double":
                    return new JsonData(this.decodeDouble());
                case "string":
                    return new JsonData(this.decodeString());
                default:
                    return this.decodeObject(type, proto);
            }
        }

        //Decode the user-defined object type in message.
        private JsonNode decodeObject(string type, JsonNode proto)
        {
            if (proto != null)
            {
                JsonNode __messages = null;
                if (proto.TryGetValue("__messages", out __messages))
                {
                    JsonNode _type = null;
                    if (__messages.TryGetValue(type, out _type) || protos.TryGetValue("message " + type, out _type))
                    {
                        int l = (int)Decoder.decodeUInt32(this.getBytes());
                        JsonNode msg = new JsonClass();
                        return this.decodeMsg(msg, _type, this.offset + l);
                    }
                }
            }
            return new JsonClass();
        }

        //Decode string type.
        private string decodeString()
        {
            int length = (int)Decoder.decodeUInt32(this.getBytes());
            string msg_string = Encoding.UTF8.GetString(this.buffer, this.offset, length);
            this.offset += length;
            return msg_string;
        }

        //Decode double type.
        private double decodeDouble()
        {
            double msg_double = BitConverter.Int64BitsToDouble((long)this.ReadRawLittleEndian64());
            this.offset += 8;
            return msg_double;
        }

        //Decode float type
        private float decodeFloat()
        {
            float msg_float = BitConverter.ToSingle(this.buffer, this.offset);
            this.offset += 4;
            return msg_float;
        }

        //Read long in littleEndian
        private ulong ReadRawLittleEndian64()
        {
            ulong b1 = buffer[this.offset];
            ulong b2 = buffer[this.offset + 1];
            ulong b3 = buffer[this.offset + 2];
            ulong b4 = buffer[this.offset + 3];
            ulong b5 = buffer[this.offset + 4];
            ulong b6 = buffer[this.offset + 5];
            ulong b7 = buffer[this.offset + 6];
            ulong b8 = buffer[this.offset + 7];
            return b1 | (b2 << 8) | (b3 << 16) | (b4 << 24)
                  | (b5 << 32) | (b6 << 40) | (b7 << 48) | (b8 << 56);
        }

        //Get the type and tag.
        private Dictionary<string, int> getHead()
        {
            int tag = (int)Decoder.decodeUInt32(this.getBytes());
            Dictionary<string, int> head = new Dictionary<string, int>();
            head.Add("type", tag & 0x7);
            head.Add("tag", tag >> 3);
            return head;
        }

        //Get bytes.
        private byte[] getBytes()
        {
            List<byte> arrayList = new List<byte>();
            int pos = this.offset;
            byte b;
            do
            {
                b = this.buffer[pos];
                arrayList.Add(b);
                pos++;
            } while (b >= 128);
            this.offset = pos;
            int length = arrayList.Count;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = arrayList[i];
            }
            return bytes;
        }
    }
}