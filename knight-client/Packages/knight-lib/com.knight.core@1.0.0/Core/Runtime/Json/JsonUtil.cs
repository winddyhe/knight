using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using Unity.Mathematics;
using UnityEngine;

namespace Knight.Core
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            JObject obj = new JObject
            {
                { "x", value.x },
                { "y", value.y },
                { "z", value.z }
            };
            obj.WriteTo(writer);
        }

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new Vector3((float)obj["x"], (float)obj["y"], (float)obj["z"]);
        }
    }

    public class Uint4Converter : JsonConverter<uint4>
    {
        public override void WriteJson(JsonWriter writer, uint4 value, JsonSerializer serializer)
        {
            JObject obj = new JObject()
            {
                { "x", value.x },
                { "y", value.y },
                { "z", value.z },
                { "w", value.w }
            };
            obj.WriteTo(writer);
        }

        public override uint4 ReadJson(JsonReader reader, Type objectType, uint4 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);
            return new uint4((uint)obj["x"], (uint)obj["y"], (uint)obj["z"], (uint)obj["w"]);
        }
    }
}
