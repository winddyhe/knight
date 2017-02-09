using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Core.WindJson;

namespace Pomelo.Protobuf
{
    public class MsgEncoder
    {
        private JsonNode protos { set; get; }//The message format(like .proto file)
        private Encoder encoder { set; get; }
        private Util util { set; get; }

        public MsgEncoder(JsonNode protos)
        {
            if (protos == null) protos = new JsonNode();

            this.protos = protos;
            this.util = new Util();
        }

        /// <summary>
        /// Encode the message from server.
        /// </summary>
        /// <param name='route'>
        /// Route.
        /// </param>
        /// <param name='msg'>
        /// Message.
        /// </param>
        public byte[] encode(string route, JsonNode msg)
        {
            byte[] returnByte = null;
            JsonNode proto = null;
            if (this.protos.TryGetValue(route, out proto))
            {
                if (!checkMsg(msg, proto))
                {
                    return null;
                }
                int length = Encoder.byteLength(msg.ToString()) * 2;
                int offset = 0;
                byte[] buff = new byte[length];
                offset = encodeMsg(buff, offset, proto, msg);
                returnByte = new byte[offset];
                for (int i = 0; i < offset; i++)
                {
                    returnByte[i] = buff[i];
                }
            }
            return returnByte;
        }

        /// <summary>
        /// Check the message.
        /// </summary>
        private bool checkMsg(JsonNode msg, JsonNode proto)
        {
            ICollection<string> protoKeys = proto.Keys;
            foreach (string key in protoKeys)
            {
                JsonNode value = proto[key];
                JsonNode proto_option = null;
                if (value.TryGetValue("option", out proto_option))
                {
                    switch (proto_option.AsString)
                    {
                        case "required":
                            if (!msg.ContainsKey(key))
                            {
                                return false;
                            }
                            else
                            {

                            }
                            break;
                        case "optional":
                            JsonNode value_type = null;

                            JsonNode messages = proto["__messages"];

                            value_type = value["type"];

                            if (msg.ContainsKey(key))
                            {
                                JsonNode value_proto = null;

                                if (messages.TryGetValue(value_type.AsString, out value_proto) || protos.TryGetValue("message " + value_type.AsString, out value_proto))
                                {
                                    checkMsg(msg[key], value_proto);
                                }
                            }
                            break;
                        case "repeated":
                            JsonNode msg_name = null;
                            JsonNode msg_type = null;
                            if (value.TryGetValue("type", out value_type) && msg.TryGetValue(key, out msg_name))
                            {
                                if ((proto["__messages"]).TryGetValue(value_type.AsString, out msg_type) || protos.TryGetValue("message " + value_type.AsString, out msg_type))
                                {
                                    JsonArray o = msg_name as JsonArray;
                                    for (int i = 0; i < o.Count; i++)
                                    {
                                        if (!checkMsg(o[i], msg_type))
                                        {
                                            return false;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Encode the message.
        /// </summary>
        private int encodeMsg(byte[] buffer, int offset, JsonNode proto, JsonNode msg)
        {
            ICollection<string> msgKeys = msg.Keys;
            foreach (string key in msgKeys)
            {
                JsonNode value = null;
                if (proto.TryGetValue(key, out value))
                {
                    JsonNode value_option = null;
                    if (value.TryGetValue("option", out value_option))
                    {
                        switch (value_option.AsString)
                        {
                            case "required":
                            case "optional":
                                JsonNode value_type = null, value_tag = null;
                                if (value.TryGetValue("type", out value_type) && value.TryGetValue("tag", out value_tag))
                                {
                                    offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.AsString, value_tag.AsInt));
                                    offset = this.encodeProp(msg[key], value_type.AsString, offset, buffer, proto);
                                }
                                break;
                            case "repeated":
                                JsonNode msg_key = null;
                                if (msg.TryGetValue(key, out msg_key))
                                {
                                    if (msg_key.Count > 0)
                                    {
                                        offset = encodeArray(msg_key as JsonArray, value, offset, buffer, proto);
                                    }
                                }
                                break;
                        }
                    }

                }
            }
            return offset;
        }

        /// <summary>
        /// Encode the array type.
        /// </summary>
        private int encodeArray(JsonArray msg, JsonNode value, int offset, byte[] buffer, JsonNode proto)
        {
            JsonNode value_type = null, value_tag = null;
            if (value.TryGetValue("type", out value_type) && value.TryGetValue("tag", out value_tag))
            {
                if (this.util.isSimpleType(value_type.AsString))
                {
                    offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.AsString, value_tag.AsInt));
                    offset = this.writeBytes(buffer, offset, Encoder.encodeUInt32((uint)msg.Count));

                    for (int i = 0; i < msg.Count; i++)
                    {
                        offset = this.encodeProp(msg[i], value_type.AsString, offset, buffer, null);
                    }
                }
                else
                {
                    for (int i = 0; i < msg.Count; i++)
                    {
                        offset = this.writeBytes(buffer, offset, this.encodeTag(value_type.AsString, value_tag.AsInt));
                        offset = this.encodeProp(msg[i], value_type.AsString, offset, buffer, proto);
                    }
                }
            }
            return offset;
        }

        /// <summary>
        /// Encode each item in message.
        /// </summary>
        private int encodeProp(JsonNode value, string type, int offset, byte[] buffer, JsonNode proto)
        {
            switch (type)
            {
                case "uInt32":
                    this.writeUInt32(buffer, ref offset, value);
                    break;
                case "int32":
                case "sInt32":
                    this.writeInt32(buffer, ref offset, value);
                    break;
                case "float":
                    this.writeFloat(buffer, ref offset, value);
                    break;
                case "double":
                    this.writeDouble(buffer, ref offset, value);
                    break;
                case "string":
                    this.writeString(buffer, ref offset, value);
                    break;
                default:
                    JsonNode __messages = null;
                    JsonNode __message_type = null;

                    if (proto.TryGetValue("__messages", out __messages))
                    {
                        if (__messages.TryGetValue(type, out __message_type) || protos.TryGetValue("message " + type, out __message_type))
                        {
                            byte[] tembuff = new byte[Encoder.byteLength(value.ToString()) * 3];
                            int length = 0;
                            length = this.encodeMsg(tembuff, length, __message_type, value);
                            offset = writeBytes(buffer, offset, Encoder.encodeUInt32((uint)length));
                            for (int i = 0; i < length; i++)
                            {
                                buffer[offset] = tembuff[i];
                                offset++;
                            }
                        }
                    }
                    break;
            }
            return offset;
        }

        //Encode string.
        private void writeString(byte[] buffer, ref int offset, object value)
        {
            int le = Encoding.UTF8.GetByteCount(value.ToString());
            offset = writeBytes(buffer, offset, Encoder.encodeUInt32((uint)le));
            byte[] bytes = Encoding.UTF8.GetBytes(value.ToString());
            this.writeBytes(buffer, offset, bytes);
            offset += le;
        }

        //Encode double.
        private void writeDouble(byte[] buffer, ref int offset, object value)
        {
            WriteRawLittleEndian64(buffer, offset, (ulong)BitConverter.DoubleToInt64Bits(double.Parse(value.ToString())));
            offset += 8;
        }

        //Encode float.
        private void writeFloat(byte[] buffer, ref int offset, object value)
        {
            this.writeBytes(buffer, offset, Encoder.encodeFloat(float.Parse(value.ToString())));
            offset += 4;
        }

        ////Encode UInt32.
        private void writeUInt32(byte[] buffer, ref int offset, JsonNode value)
        {
            offset = writeBytes(buffer, offset, Encoder.encodeUInt32(value.ToString()));
        }

        //Encode Int32
        private void writeInt32(byte[] buffer, ref int offset, JsonNode value)
        {
            offset = writeBytes(buffer, offset, Encoder.encodeSInt32(value.ToString()));
        }

        //Write bytes to buffer.
        private int writeBytes(byte[] buffer, int offset, byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                buffer[offset] = bytes[i];
                offset++;
            }
            return offset;
        }

        //Encode tag.
        private byte[] encodeTag(string type, int tag)
        {
            int flag = this.util.containType(type);
            return Encoder.encodeUInt32((uint)(tag << 3 | flag));
        }


        private void WriteRawLittleEndian64(byte[] buffer, int offset, ulong value)
        {
            buffer[offset++] = ((byte)value);
            buffer[offset++] = ((byte)(value >> 8));
            buffer[offset++] = ((byte)(value >> 16));
            buffer[offset++] = ((byte)(value >> 24));
            buffer[offset++] = ((byte)(value >> 32));
            buffer[offset++] = ((byte)(value >> 40));
            buffer[offset++] = ((byte)(value >> 48));
            buffer[offset++] = ((byte)(value >> 56));
        }
    }
}