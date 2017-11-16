//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Core.WindJson
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonEnableAttribute : Attribute { }

    /// <summary>
    /// 使用词法分析和语法分析来解析Json数据
    /// </summary>
    public class JsonParser
    {
        public enum JsonSymbolType
        {
            Unknown = 0,    // 未知
            ObjStart,       // '{'
            ObjEnd,         // '}'
            ArrayStart,     // '['
            ArrayEnd,       // ']'
            ObjSplit,       // ','
            ElementSplit,   // ':'
            Key,            // 名字：字符串
            Value,          // 值类型：整数、实数、字符串、true、false、null
            Element,        // 元素
        }

        public class JsonSymbolItem
        {
            public string           value;
            public JsonSymbolType   type;
            public JsonNode         node;

            public JsonSymbolItem() { }
            public JsonSymbolItem(JsonSymbolItem rItem)
            {
                this.value = rItem.value;
                this.type  = rItem.type;
                this.node  = rItem.node;
            }
        }
        
        /// <summary>
        /// json的原始的字符串数据
        /// </summary>
        public string originData;
        /// <summary>
        /// JsonParser是否合法
        /// </summary>
        public bool   isValid;

        public JsonParser(string rOriginData)
        {
            this.originData = rOriginData.Trim();
            isValid = true;
        }

        /// <summary>
        /// 转换为JsonNode的静态方法
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        public static JsonNode Parse(string jsonStr)
        {
            JsonParser rJsonParser = new JsonParser(jsonStr);
            return rJsonParser.Parser();
        }

        /// <summary>
        /// 将json字符串归一化，将有用的信息保留，让其更加紧凑
        /// </summary>
        public string PretreatmentProc()
        {
            string temp = "";
            //string tempWord = "";

            int i = 0;
            int end = 0;
            while(i < originData.Length)
            {
                //跳过那些无用的字符 ' ', '\t', '\r' '\n'
                if (!string.IsNullOrEmpty(LexicalAnalysis.isSpecialSymbol(originData, i, ref end)))
                {
                    i = end;
                    continue;
                }

                //跳过注释
                if (!string.IsNullOrEmpty(/*tempWord = */LexicalAnalysis.isComment(originData, i, ref end)))
                {
                    //Debug.Log(tempWord);
                    i = end;
                    continue;
                }
                temp += originData[i];
                i++;
            }

            return temp;
        }

        /// <summary>
        /// 解析Json
        /// </summary>
        public JsonNode Parser()
        {
            this.isValid = true;
            int end = 0;
            int i = 0;

            JsonSymbolItem rCurSymbol = null;
            JsonSymbolItem rLastSymbol = null;
            
            Stack<JsonSymbolItem> rNodeStack = new Stack<JsonSymbolItem>();
            while (i < this.originData.Length)
            {
                //跳过那些无用的字符 ' ', '\t', '\r' '\n' 注释
                if (!string.IsNullOrEmpty(LexicalAnalysis.isSpecialSymbol(originData, i, ref end)) || !string.IsNullOrEmpty(LexicalAnalysis.isComment(originData, i, ref end)))
                {
                    i = end;
                    continue;
                }

                rCurSymbol = buildSymbolItem(rLastSymbol, i, ref end);
                if (rCurSymbol != null)
                {
                    switch (rCurSymbol.type)
                    {
                        case JsonSymbolType.Unknown:
                            Debug.LogError("Json format error.");
                            break;
                        case JsonSymbolType.ObjStart:
                            rCurSymbol.node = new JsonClass();
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case JsonSymbolType.ObjEnd:
                            JsonNode rObject0 = new JsonClass();
                            while(rNodeStack.Count != 0 && rNodeStack.Peek().type != JsonSymbolType.ObjStart)
                            {
                                var rTopSymbol = rNodeStack.Pop();
                                if (rTopSymbol.type == JsonSymbolType.ObjSplit)
                                {
                                    continue;
                                }
                                else if (rTopSymbol.type == JsonSymbolType.Element)
                                {
                                    rObject0.AddHead(rTopSymbol.node.Key, rTopSymbol.node[rTopSymbol.node.Key]);
                                }
                            }
                            rNodeStack.Pop();
                            var rSymbol0 = new JsonSymbolItem();
                            rSymbol0.type = JsonSymbolType.Value;
                            rSymbol0.node = rObject0;
                            rSymbol0.value = rObject0.ToString();
                            Generate_ElementSymbol(ref rNodeStack, rSymbol0);
                            break;
                        case JsonSymbolType.ArrayStart:
                            rCurSymbol.node = new JsonArray();
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case JsonSymbolType.ArrayEnd:
                            JsonNode rArray = new JsonArray();
                            while (rNodeStack.Peek().type != JsonSymbolType.ArrayStart)
                            {
                                var rTopSymbol = rNodeStack.Pop();
                                if (rTopSymbol.type == JsonSymbolType.ObjSplit)
                                {
                                    continue;
                                }
                                else if (rTopSymbol.type == JsonSymbolType.Element)
                                {
                                    rArray.AddHead(rTopSymbol.node);
                                }
                            }
                            rNodeStack.Pop();
                            var rSymbol = new JsonSymbolItem();
                            rSymbol.type = JsonSymbolType.Value;
                            rSymbol.node = rArray;
                            rSymbol.value = rArray.ToString();
                            Generate_ElementSymbol(ref rNodeStack, rSymbol);
                            break;
                        case JsonSymbolType.ObjSplit:
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case JsonSymbolType.ElementSplit:
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case JsonSymbolType.Key:
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case JsonSymbolType.Value:
                            Generate_ElementSymbol(ref rNodeStack, rCurSymbol);
                            break;
                        default:
                            break;
                    }
                    i = end;
                    rLastSymbol = rCurSymbol;
                    continue;
                }
                i++;
            }
            return rNodeStack.Peek().node;
        }

        private void Generate_ElementSymbol(ref Stack<JsonSymbolItem> rNodeStack, JsonSymbolItem rCurSymbol)
        {
            if (rNodeStack.Count == 0)
            {
                rNodeStack.Push(rCurSymbol);
                return;
            }

            var rSymbol1 = rNodeStack.Pop();
            var rSymbol4 = new JsonSymbolItem();
            if (rSymbol1.type == JsonSymbolType.ObjSplit || rSymbol1.type == JsonSymbolType.ArrayStart)
            {
                rNodeStack.Push(rSymbol1);
                rSymbol4.type = JsonSymbolType.Element;
                rSymbol4.node = rCurSymbol.node;
                rSymbol4.value = rSymbol4.node.ToString();
                rNodeStack.Push(rSymbol4);
            }
            else if (rSymbol1.type == JsonSymbolType.ElementSplit)
            {
                var rSymbol2 = rNodeStack.Count == 0 ? null : rNodeStack.Pop();
                if (rSymbol2 != null && rSymbol2.type == JsonSymbolType.Key)
                {
                    rSymbol4.type = JsonSymbolType.Element;
                    rSymbol4.node = new JsonClass();
                    rSymbol4.node.Add(rSymbol2.value, rCurSymbol.node);
                    rSymbol4.value = rSymbol4.node.ToString();
                    rNodeStack.Push(rSymbol4);
                }
            }
            else
            {
                Debug.LogError("Json grammar error!");
            }
        }

        private JsonSymbolItem buildSymbolItem(JsonSymbolItem rLastSymbol, int begin, ref int end)
        {
            if (originData[begin] == '{')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = "{", type = JsonSymbolType.ObjStart };
            }
            else if (originData[begin] == '}')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = "}", type = JsonSymbolType.ObjEnd };
            }
            else if (originData[begin] == '[')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = "[", type = JsonSymbolType.ArrayStart };
            }
            else if (originData[begin] == ']')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = "]", type = JsonSymbolType.ArrayEnd };
            }
            else if (originData[begin] == ',')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = ",", type = JsonSymbolType.ObjSplit };
            }
            else if (originData[begin] == ':')
            {
                end = begin + 1;
                return new JsonSymbolItem() { value = ":", type = JsonSymbolType.ElementSplit };
            }

            string tempWord = "";
            //如果是关键字、数字或者字符串
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isKeyword(originData, begin, ref end)))
            {
                JsonSymbolItem rSymbol = new JsonSymbolItem() { value = tempWord, type = JsonSymbolType.Value, node = new JsonData(tempWord) };
                LexicalAnalysis.isSpecialSymbol(originData, end, ref end);
                if (originData[end] == ':')
                {
                    rSymbol.type = JsonSymbolType.Key;
                    rSymbol.node = null;
                }
                return rSymbol;
            }
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isDigit(originData, begin, ref end)))
            {
                JsonSymbolItem rSymbol = new JsonSymbolItem() { value = tempWord, type = JsonSymbolType.Value, node = new JsonData(tempWord) };
                LexicalAnalysis.isSpecialSymbol(originData, end, ref end);
                if (originData[end] == ':')
                {
                    rSymbol.type = JsonSymbolType.Key;
                    rSymbol.node = null;
                }
                return rSymbol;
            }
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isString(originData, begin, ref end)))
            {
                tempWord = tempWord.Substring(1, tempWord.Length - 2);
                JsonSymbolItem rSymbol = new JsonSymbolItem() { value = tempWord, type = JsonSymbolType.Value, node = new JsonData(tempWord) };
                LexicalAnalysis.isSpecialSymbol(originData, end, ref end);
                if (originData[end] == ':')
                {
                    rSymbol.type = JsonSymbolType.Key;
                    rSymbol.node = null;
                }
                return rSymbol;
            }
            //Debug.Log(string.Format("Json parse symbol item error! LastSymbol = {0}",
            //               rLastSymbol != null ? rLastSymbol.value : "null"));
            isValid = false;
            return null;
        }
        
        /// <summary>
        /// 从对象到JsonNode
        /// </summary>
        public static JsonNode ToJsonNode(object rObject)
        {
            Type rType = rObject.GetType();

            JsonNode rRootNode = null;

            //如果是List
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
            else if (rType.IsArray) //如果是Array
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
                //如果是Dictionary
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
            else if (rType.IsClass) //如果是Class，获取Class所有的public的字段和属性
            {
                if (rType == typeof(string))
                {
                    rRootNode = new JsonData((string)rObject);
                }
                else
                {
                    rRootNode = new JsonClass();
                    // 所有公共的属性
                    PropertyInfo[] rPropInfos = rType.GetProperties(ReflectionAssist.flags_public);
                    for (int i = 0; i < rPropInfos.Length; i++)
                    {
                        if (rPropInfos[i].IsDefined(typeof(JsonIgnoreAttribute), false))
                            continue;

                        object rValueObj = rPropInfos[i].GetValue(rObject, null);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rPropInfos[i].Name, rValueNode);
                    }
                    // 所有公共的字段
                    FieldInfo[] rFieldInfos = rType.GetFields(ReflectionAssist.flags_public);
                    for (int i = 0; i < rFieldInfos.Length; i++)
                    {
                        if (rFieldInfos[i].IsDefined(typeof(JsonIgnoreAttribute), false))
                            continue;

                        object rValueObj = rFieldInfos[i].GetValue(rObject);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rFieldInfos[i].Name, rValueNode);
                    }

                    // 所有预定义的序列化属性的private的字段
                    rPropInfos = rType.GetProperties(ReflectionAssist.flags_nonpublic);
                    for (int i = 0; i < rPropInfos.Length; i++)
                    {
                        if (!rPropInfos[i].IsDefined(typeof(JsonEnableAttribute), false))
                            continue;

                        object rValueObj = rPropInfos[i].GetValue(rObject, null);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rPropInfos[i].Name, rValueNode);
                    }
                    rFieldInfos = rType.GetFields(ReflectionAssist.flags_nonpublic);
                    for (int i = 0; i < rFieldInfos.Length; i++)
                    {
                        if (!rFieldInfos[i].IsDefined(typeof(JsonEnableAttribute), false))
                            continue;
                        
                        object rValueObj = rFieldInfos[i].GetValue(rObject);
                        JsonNode rValueNode = ToJsonNode(rValueObj);
                        rRootNode.Add(rFieldInfos[i].Name, rValueNode);
                    }
                }
            }
            else if (rType.IsPrimitive) //如果是实例
            {
                if (rType == typeof(int))
                    rRootNode = new JsonData((int)rObject);
                else if (rType == typeof(uint))
                    rRootNode = new JsonData((uint)rObject);
                else if (rType == typeof(long))
                    rRootNode = new JsonData((long)rObject);
                else if (rType == typeof(float))
                    rRootNode = new JsonData((float)rObject);
                else if (rType == typeof(double))
                    rRootNode = new JsonData((double)rObject);
                else if (rType == typeof(bool))
                    rRootNode = new JsonData((bool)rObject);
                else if (rType == typeof(string))
                    rRootNode = new JsonData((string)rObject);
                else
                    Debug.LogError(string.Format("Type = {0}, 不支持序列化的变量类型!", rObject.GetType()));
            }
            return rRootNode;
        }

        public static object ToObject(JsonNode rJsonNode, Type rType)
        {
            return rJsonNode.ToObject(rType);
        }

        public static object ToList(JsonNode rJsonNode, Type rType, Type rElemType)
        {
            return rJsonNode.ToList(rType, rElemType);
        }

        public static object ToDict(JsonNode rJsonNode, Type rDictType, Type rKeyType, Type rValueType)
        {
            return rJsonNode.ToDict(rDictType, rKeyType, rValueType);
        }
    }
}