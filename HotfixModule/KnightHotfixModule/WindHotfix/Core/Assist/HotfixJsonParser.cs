//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Core.WindJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WindHotfix.Core
{
    public static class HotfixJsonParser
    {
        public static JsonNode Parse(string rJsonText)
        {
            return JsonParser.Parse(rJsonText);
        }

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
        
        public static JsonNode ToJsonNode(object rObject)
        {
            Type rType = rObject.GetType();
            JsonNode rRootNode = null;
            
            if (rType.IsGenericType && typeof(IList).IsAssignableFrom(rType.GetGenericTypeDefinition()))
            {
                rRootNode = new JsonArray();
                IList rListObj = (IList)rObject;
                foreach (var rItem in rListObj)
                {
                    JsonNode rNode = ToJsonNode(rItem);
                    rRootNode.Add(rNode);
                }
            }
            else if (rType.IsArray)
            {
                rRootNode = new JsonArray();
                Array rArrayObj = (Array)rObject;
                foreach (var rItem in rArrayObj)
                {
                    JsonNode rNode = ToJsonNode(rItem);
                    rRootNode.Add(rNode);
                }
            }
            else if (rType.IsGenericType && typeof(IDictionary).IsAssignableFrom(rType.GetGenericTypeDefinition()))
            {
                rRootNode = new JsonClass();
                IDictionary rDictObj = (IDictionary)rObject;
                foreach (var rKey in rDictObj.Keys)
                {
                    JsonNode rValueNode = ToJsonNode(rDictObj[rKey]);
                    rRootNode.Add(rKey.ToString(), rValueNode);
                }
            }
            else if (rType.IsGenericType && typeof(IDict).IsAssignableFrom(rType.GetGenericTypeDefinition()))
            {
                rRootNode = new JsonClass();
                IDict rDictObj = (IDict)rObject;
                foreach (var rItem in rDictObj.OriginCollection)
                {
                    JsonNode rValueNode = ToJsonNode(rItem.Value);
                    rRootNode.Add(rItem.Key.ToString(), rValueNode);
                }
            }
            else if (rType.IsClass)
            {
                if (rType == typeof(string))
                {
                    rRootNode = new JsonData((string)rObject);
                }
                else
                {
                    rRootNode = new JsonClass();
                    PropertyInfo[] rPropInfos = rType.GetProperties(ReflectionAssist.flags_public);
                    for (int i = 0; i < rPropInfos.Length; i++)
                    {
                        object rValueObj = rPropInfos[i].GetValue(rObject, null);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rPropInfos[i].Name, rValueNode);
                    }
                    FieldInfo[] rFieldInfos = rType.GetFields(ReflectionAssist.flags_public);
                    for (int i = 0; i < rFieldInfos.Length; i++)
                    {
                        object rValueObj = rFieldInfos[i].GetValue(rObject);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rFieldInfos[i].Name, rValueNode);
                    }
                }
            }
            else if (rType.IsPrimitive)
            {
                if (rType.Equals(typeof(int)))
                    rRootNode = new JsonData((int)rObject);
                else if (rType.Equals(typeof(uint)))
                    rRootNode = new JsonData((uint)rObject);
                else if (rType.Equals(typeof(long)))
                    rRootNode = new JsonData((long)rObject);
                else if (rType.Equals(typeof(float)))
                    rRootNode = new JsonData((float)rObject);
                else if (rType.Equals(typeof(double)))
                    rRootNode = new JsonData((double)rObject);
                else if (rType.Equals(typeof(bool)))
                    rRootNode = new JsonData((bool)rObject);
                else if (rType.Equals(typeof(string)))
                    rRootNode = new JsonData((string)rObject);
                else
                    Debug.LogError(string.Format("Type = {0}, 不支持序列化的变量类型!", rObject.GetType()));
            }
            return rRootNode;
        }
    }
}