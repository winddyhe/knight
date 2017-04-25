using Core;
using Core.WindJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindHotfix.Core
{
    public static class HotfixJsonParser
    {
        public static T ToObject<T>(JsonNode rJsonNode)
        {
            return (T)JsonParser.ToObject(rJsonNode, typeof(T));
        }

        public static List<T> ToList<T>(JsonNode rJsonNode)
        {
            return (List<T>)JsonParser.ToList(rJsonNode, typeof(List<T>), typeof(T));
        }

        public static T[] ToArray<T>(JsonNode rJsonNode)
        {
            return (T[])JsonParser.ToList(rJsonNode, typeof(T[]), typeof(T));
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(JsonNode rJsonNode)
        {
            return (Dictionary<TKey, TValue>)JsonParser.ToDict(rJsonNode, typeof(Dictionary<TKey, TValue>), typeof(TKey), typeof(TValue));
        }

        public static Dict<TKey, TValue> ToDict<TKey, TValue>(JsonNode rJsonNode)
        {
            return (Dict<TKey, TValue>)JsonParser.ToDict(rJsonNode, typeof(Dict<TKey, TValue>), typeof(TKey), typeof(TValue));
        }
    }
}