using System;
using System.Collections.Generic;
using Core.WindJson;
using Core.Serializer;
using Core;
using System.IO;

namespace Model
{
    public class SerializerPacker : IMessagePacker
    {
        public object DeserializeFrom(Type type, byte[] bytes)
        {
            var rObj = ReflectionAssist.Construct(type) as SerializerBinary;
            using (var ms = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj;
        }

        public object DeserializeFrom(Type type, byte[] bytes, int index, int count)
        {
            var rObj = ReflectionAssist.Construct(type) as SerializerBinary;
            using (var ms = new MemoryStream(bytes, index, count))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj;
        }

        public T DeserializeFrom<T>(byte[] bytes) where T : SerializerBinary
        {
            var rObj = ReflectionAssist.Construct(typeof(T)) as T;
            using (var ms = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj as T;
        }

        public T DeserializeFrom<T>(byte[] bytes, int index, int count) where T : SerializerBinary
        {
            var rObj = ReflectionAssist.Construct(typeof(T)) as T;
            using (var ms = new MemoryStream(bytes, index, count))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj as T;
        }

        public T DeserializeFrom<T>(string str) where T : SerializerBinary
        {
            var rJsonNode = JsonParser.ToJsonNode(str);
            return rJsonNode.ToObject<T>();
        }

        public object DeserializeFrom(Type type, string str)
        {
            var rJsonNode = JsonParser.Parse(str);
            return rJsonNode.ToObject(type);
        }

        public byte[] SerializeToByteArray(object obj)
        {
            var rObj = obj as SerializerBinary;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    rObj?.Serialize(bw);
                    return ms.GetBuffer();
                }
            }
        }

        public string SerializeToText(object obj)
        {
            var rJsonNode = JsonParser.ToJsonNode(obj);
            return rJsonNode.ToString();
        }
    }
}
