using Knight.Core.WindJson;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Knight.Hotfix.Core
{
    public class HotfixNetworkMessagePacker
    {
        private static readonly RecyclableMemoryStreamManager mRecyclableMSMgr = new RecyclableMemoryStreamManager();

        public object DeserializeFrom(Type rType, byte[] rBytes)
        {
            var rObj = HotfixReflectAssists.Construct(rType) as HotfixSerializerBinary;
            using (var ms = new MemoryStream(rBytes))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            return rObj;
        }

        public object DeserializeFrom(Type rType, byte[] rBytes, int nIndex, int nCount)
        {
            var rObj = HotfixReflectAssists.Construct(rType) as HotfixSerializerBinary;
            using (MemoryStream ms = mRecyclableMSMgr.GetStream("protobuf", rBytes, nIndex, nCount))
            {
                using (var br = new BinaryReader(ms))
                {
                    rObj.Deserialize(br);
                }
            }
            ISupportInitialize iSupportInitialize = rObj as ISupportInitialize;
            if (iSupportInitialize == null)
            {
                return rObj;
            }
            iSupportInitialize.EndInit();
            return rObj;
        }

        public T DeserializeFrom<T>(byte[] rBytes)
        {
            return DeserializeFrom<T>(rBytes, 0, rBytes.Length);
        }

        public T DeserializeFrom<T>(byte[] rBytes, int nIndex, int nCount)
        {
            return (T)DeserializeFrom(typeof(T), rBytes, nIndex, nCount);
        }

        public T DeserializeFrom<T>(string rStr)
        {
            JsonNode rJsonNode = HotfixJsonParser.Parse(rStr);
            return rJsonNode.ToObject<T>();
        }

        public object DeserializeFrom(Type rType, string rStr)
        {
            JsonNode rJsonNode = HotfixJsonParser.Parse(rStr);
            return rJsonNode.ToObject(rType);
        }

        public byte[] SerializeToByteArray(object rObj)
        {
            var rSerializerObj = rObj as HotfixSerializerBinary;
            using (var ms = new MemoryStream())
            {
                using (var bw = new BinaryWriter(ms))
                {
                    rSerializerObj?.Serialize(bw);
                    return ms.GetBuffer();
                }
            }
        }

        public string SerializeToText(object rObj)
        {
            JsonNode rJsonNode = HotfixJsonParser.ToJsonNode(rObj);
            return rJsonNode.ToString();
        }
    }
}
