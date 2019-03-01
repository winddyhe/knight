using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core.WindJson;
using System.IO;
using Microsoft.IO;
using Knight.Core;
using Knight.Core.Serializer;
using System.ComponentModel;

namespace Knight.Framework.Net
{
    public class NetworkMessagePacker : IMessagePacker
    {
        private static readonly RecyclableMemoryStreamManager mRecyclableMSMgr = new RecyclableMemoryStreamManager();

        public object DeserializeFrom(Type rType, byte[] rBytes)
        {
            var rObj = ReflectionAssist.Construct(rType) as SerializerBinary;
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
            var rObj = ReflectionAssist.Construct(rType) as SerializerBinary;
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
            JsonNode rJsonNode = JsonParser.Parse(rStr);
            return rJsonNode.ToObject<T>();
        }

        public object DeserializeFrom(Type rType, string rStr)
        {
            JsonNode rJsonNode = JsonParser.Parse(rStr);
            return rJsonNode.ToObject(rType);
        }

        public byte[] SerializeToByteArray(object rObj)
        {
            var rSerializerObj = rObj as SerializerBinary;
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
            JsonNode rJsonNode = JsonParser.ToJsonNode(rObj);
            return rJsonNode.ToString();
        }
    }
}