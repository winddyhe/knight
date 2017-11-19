using System;
using System.IO;
using WindHotfix.Core;

namespace WindHotfix.Net
{
    public class SerializerPacker
    {
        public object DeserializeFrom(Type type, byte[] bytes)
        {
            var rObj = HotfixReflectAssists.Construct(type) as HotfixSerializerBinary;
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
            var rObj = HotfixReflectAssists.Construct(type) as HotfixSerializerBinary;
            using (var ms = new MemoryStream(bytes, index, count))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj;
        }

        public T DeserializeFrom<T>(byte[] bytes) where T : HotfixSerializerBinary
        {
            var rObj = HotfixReflectAssists.Construct(typeof(T)) as T;
            using (var ms = new MemoryStream(bytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj as T;
        }

        public T DeserializeFrom<T>(byte[] bytes, int index, int count) where T : HotfixSerializerBinary
        {
            var rObj = HotfixReflectAssists.Construct(typeof(T)) as T;
            using (var ms = new MemoryStream(bytes, index, count))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj as T;
        }

        public T DeserializeFrom<T>(string str) where T : HotfixSerializerBinary
        {
            var rJsonNode = HotfixJsonParser.ToJsonNode(str);
            return rJsonNode.ToObject<T>();
        }

        public object DeserializeFrom(Type type, string str)
        {
            var rJsonNode = HotfixJsonParser.Parse(str);
            return rJsonNode.ToObject(type);
        }

        public byte[] SerializeToByteArray(object obj)
        {
            var rObj = obj as HotfixSerializerBinary;
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
            var rJsonNode = HotfixJsonParser.ToJsonNode(obj);
            return rJsonNode.ToString();
        }
    }
}
