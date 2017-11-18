//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Core.WindJson
{
    public class JsonNode
    {
        public virtual string       Value                                   { get; set; }
        public virtual string       Key                                     { get; set; }
        public virtual int          Count                                   { get; private set; }

        public virtual JsonNode     this[int nIndex]                        { get { return null; } set { } }
        public virtual JsonNode     this[string rKey]                       { get { return null; } set { } }
        public virtual JsonNode     Node                                    { get; set; }
        
        public virtual void         Add(string rKey, JsonNode rItem)        {}
        public virtual void         Add(JsonNode rItem)                     {}

        public virtual void         AddHead(string rKey, JsonNode rItem)    {}
        public virtual void         AddHead(JsonNode rItem)                 {}

        public virtual JsonNode     Remove(string rKey)                     { return null;  }
        public virtual JsonNode     Remove(int nIndex)                      { return null;  }
        public virtual JsonNode     Remove(JsonNode rNode)                  { return rNode; }

        public virtual List<string> Keys                                    { get { return new List<string>(); } }
        public virtual bool         ContainsKey(string rKey)                { return false; }

        public override string      ToString()                              { return base.ToString();    }
        public virtual object       ToObject(Type rType)                    { return null;               }
        public T                    ToObject<T>()                           { return (T)ToObject(typeof(T));              }
        public List<T>              ToList<T>()                             { return (List<T>)ToObject(typeof(List<T>));  }
        public T[]                  ToArray<T>()                            { return (T[])ToObject(typeof(T[]));          }

        public virtual object       ToList(Type rListType, Type rElemType)  { return null; }
        public virtual object       ToDict(Type rDictType, Type rKeyType, Type rValueType) { return null; }

        public virtual bool TryGetValue(string key, out JsonNode value)
        {
            value = null;
            return false;
        }

        public Dict<TKey, TValue> ToDict<TKey, TValue>() 
        { 
            return (Dict<TKey, TValue>)ToObject(typeof(Dict<TKey, TValue>));  
        }
        public Dictionary<TKey, TValue> ToDictionary<TKey, TValue>() 
        {
            return (Dictionary<TKey, TValue>)ToObject(typeof(Dictionary<TKey, TValue>));
        }

        public virtual int AsInt
        {
            get { return CastInt(Value);    }
            set { Value = value.ToString(); }
        }

        public virtual long AsLong
        {
            get { return CastLong(Value);   }
            set { Value = value.ToString(); }
        }

        public virtual float AsFloat
        {
            get { return CastFloat(Value);  }
            set { Value = value.ToString(); }
        }

        public virtual double AsDouble
        {
            get { return CastDouble(Value); }
            set { Value = value.ToString(); }
        }

        public virtual bool AsBool
        {
            get { return CastBool(Value);   }
            set { Value = value.ToString(); }
        }

        public virtual string AsString
        {
            get { return Value;  }
            set { Value = value; }
        }

        public int CastInt(string value)
        {
            int re = 0;
            if (int.TryParse(value, out re)) return re;
            Model.Log.Error(string.Format("Value: {0} is not int type.", value));
            return re;
        }

        public long CastLong(string value)
        {
            long re = 0;
            if (long.TryParse(value, out re)) return re;
            Model.Log.Error(string.Format("Value: {0} is not int type.", value));
            return re;
        }

        public float CastFloat(string value)
        {
            float re = 0;
            if (float.TryParse(value, out re)) return re;
            Model.Log.Error(string.Format("Value: {0} is not int type.", value));
            return re;
        }

        public double CastDouble(string value)
        {
            double re = 0;
            if (double.TryParse(value, out re)) return re;
            Model.Log.Error(string.Format("Value: {0} is not int type.", value));
            return re;
        }

        public bool CastBool(string value)
        {
            if (value == "false" || value == "true")
                return value == "false" ? false : true;
            else
                return CastInt(value) == 0 ? false : true;
        }

        public object CastEnum(Type type, string value)
        {
            //如果enum是数字，那么返回数字
            int re = 0;
            if (int.TryParse(value, out re)) return re;

            //如果不是数字，而是字符串，直接转换为enum
            return Enum.Parse(type, value, true);
        }
    }

    public class JsonArray : JsonNode, IEnumerable
    {
        private List<JsonNode> list = new List<JsonNode>();

        public override JsonNode this[int nIndex]
        {
            get
            {
                if (nIndex >= 0 && nIndex < Count)  return list[nIndex];
                Model.Log.Error(string.Format("Index out of size limit, Index = {0}, Count = {1}", nIndex, Count));
                return null;
            }
            set
            {
                if (nIndex >= Count)
                    list.Add(value);
                else if (nIndex >= 0 && nIndex < Count)
                    list[nIndex] = value;
            }
        }

        public override int Count { get { return list.Count; } }

        public override void Add(JsonNode rItem)
        {
            if (!list.Contains(rItem))
                list.Add(rItem);
        }

        public override void AddHead(JsonNode rItem)
        {
            if (!list.Contains(rItem))
                list.Insert(0, rItem);
        }

        public override JsonNode Remove(int nIndex)
        {
            if (nIndex < 0 || nIndex >= Count)
                return null;
            JsonNode tmp = list[nIndex];
            list.RemoveAt(nIndex);
            return tmp;
        }

        public override JsonNode Remove(JsonNode rNode)
        {
            list.Remove(rNode);
            return rNode;
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var rNode in list)
            {
                yield return rNode;
            }
        }

        public override string ToString()
        {
            string jsonStr = "[";
            for (int i = 0; i < list.Count - 1; i++)
            {
                jsonStr += list[i].ToString();
                jsonStr += ",";
            }
			jsonStr += list.Count == 0 ? "" : list[list.Count - 1].ToString();
            jsonStr += "]";
            return jsonStr;
        }

        public override object ToObject(Type rType)
        {
            if (rType.IsArray)
            {
                Array rObject = Array.CreateInstance(rType.GetElementType(), this.Count);
                Type rArrayElemType = rType.GetElementType();
                for (int i = 0; i < this.Count; i++)
                {
                    object rValue = this.list[i].ToObject(rArrayElemType);
                    rObject.SetValue(rValue, i);
                }
                return rObject;
            }
            else if (rType.IsGenericType && typeof(IList).IsAssignableFrom(rType.GetGenericTypeDefinition()))  //是否为泛型
            {
                IList rObject = (IList)Activator.CreateInstance(rType);
                Type[] rArgsTypes = rType.GetGenericArguments();
                for (int i = 0; i < this.Count; i++)
                {
                    var rElemType = rArgsTypes[0];
                    object rValue = this.list[i].ToObject(rElemType);
                    rObject.Add(rValue);
                }
                return rObject;
            }
            return null;
        }

        public override object ToList(Type rListType, Type rElemType)
        {
            var rCLRType = rListType;
            if (rCLRType.IsArray)
            {
                Array rObject = Array.CreateInstance(rCLRType.GetElementType(), this.Count);
                for (int i = 0; i < this.Count; i++)
                {
                    object rValue = this.list[i].ToObject(rElemType);
                    rObject.SetValue(rValue, i);
                }
                return rObject;
            }
            else if (rCLRType.IsGenericType && typeof(IList).IsAssignableFrom(rCLRType.GetGenericTypeDefinition()))  //是否为泛型
            {
                IList rObject = (IList)Activator.CreateInstance(rListType);
                Type[] rArgsTypes = rCLRType.GetGenericArguments();
                for (int i = 0; i < this.Count; i++)
                {
                    object rValue = this.list[i].ToObject(rElemType);
                    rObject.Add(rValue);
                }
                return rObject;
            }
            return null;
        }
    }

    public class JsonClass : JsonNode, IEnumerable
    {
        private Dict<string, JsonNode> dict = new Dict<string, JsonNode>();

        public override JsonNode this[string rKey]
        {
            get
            {
                JsonNode rNode = null;
                dict.TryGetValue(rKey, out rNode);
                return rNode;
            }
            set
            {
                if (dict.ContainsKey(rKey))
                    dict[rKey] = value;
                else
                    dict.Add(rKey, value);
            }
        }

        public override List<string> Keys
        {
            get
            {
                List<string> rKeys = new List<string>();
                foreach (var rItem in dict)
                {
                    rKeys.Add(rItem.Key);
                }
                return rKeys;
            }
        }

        public override string Key
        {
            get
            {
                if (dict.Count == 0) return "";
                return dict.FirstKey();
            }
            set
            {
            }
        }

        public override bool ContainsKey(string rKey)
        {
            return dict.ContainsKey(rKey);
        }

        public override int Count { get { return dict.Count; } }

        public override void Add(string rKey, JsonNode rItem)
        {
            if (!string.IsNullOrEmpty(rKey))
            {
                if (dict.ContainsKey(rKey))
                    dict[rKey] = rItem;
                else
                    dict.Add(rKey, rItem);
            }
            else
            {
                Model.Log.Error("JsonClass dict cannot Add empty string key.");
            }
        }

        public override void AddHead(string rKey, JsonNode rItem)
        {
            if (!string.IsNullOrEmpty(rKey))
            {
                if (dict.ContainsKey(rKey))
                    dict[rKey] = rItem;
                else
                {
                    var rTempdict = new Dict<string, JsonNode>();
                    rTempdict.Add(rKey, rItem);
                    foreach (var rPair in dict)
                    {
                        rTempdict.Add(rPair.Key, rPair.Value);
                    }
                    dict = rTempdict;
                }
            }
            else
            {
                Model.Log.Error("JsonClass dict cannot Add empty string key.");
            }
        }

        public override JsonNode Remove(string rKey)
        {
            if (!dict.ContainsKey(rKey)) return null;
            JsonNode rNode = dict[rKey];
            dict.Remove(rKey);
            return rNode;
        }

        public override JsonNode Remove(JsonNode rNode)
        {
            return base.Remove(rNode);
        }

        public IEnumerator GetEnumerator()
        {
            foreach (var rItem in dict)
                yield return rItem;
        }

        public override string ToString()
        {
            string jsonStr = "{";
            int i = 0;
            foreach (var rItem in dict)
            {
                jsonStr += "\"" + rItem.Key + "\":" + rItem.Value.ToString();
                if (i < Count - 1) jsonStr += ",";
                i++;
            }
            jsonStr += "}";
            return jsonStr;
        }

        public override object ToObject(Type rType)
        {
            if (rType.IsGenericType && typeof(IDictionary).IsAssignableFrom(rType.GetGenericTypeDefinition()))
            {
                // 特殊处理IDictionary<,>类型
                IDictionary rObject = (IDictionary)ReflectionAssist.CreateInstance(rType, ReflectionAssist.flags_all);
                Type[] rArgsTypes = rType.GetGenericArguments();
                foreach (var rItem in this.dict)
                {
                    object rKey = GetKey_ByString(rArgsTypes[0], rItem.Key);
                    object rValue = rItem.Value.ToObject(rArgsTypes[1]);
                    rObject.Add(rKey, rValue);
                }
                return rObject;
            }
            else if (rType.IsGenericType && typeof(IDict).IsAssignableFrom(rType.GetGenericTypeDefinition()))
            {
                // 特殊处理IDict<,>的类型
                IDict rObject = (IDict)ReflectionAssist.CreateInstance(rType, ReflectionAssist.flags_all);
                Type[] rArgsTypes = rType.GetGenericArguments();
                foreach (var rItem in this.dict)
                {
                    object rKey = GetKey_ByString(rArgsTypes[0], rItem.Key);
                    object rValue = rItem.Value.ToObject(rArgsTypes[1]);
                    rObject.AddObject(rKey, rValue);
                }
                return rObject;
            }
            else if (rType.IsClass)
            {
                BindingFlags rBindFlags = ReflectionAssist.flags_all;
                object rObject = ReflectionAssist.CreateInstance(rType, rBindFlags);
                foreach (var rItem in this.dict)
                {
                    Type rMemberType = null;
                    FieldInfo rFieldInfo = rType.GetField(rItem.Key, rBindFlags);
                    if (rFieldInfo != null)
                    {
                        rMemberType = rFieldInfo.FieldType;
                        object rValueObj = rItem.Value.ToObject(rMemberType);
                        rFieldInfo.SetValue(rObject, rValueObj);
                        continue;
                    }
                    PropertyInfo rPropInfo = rType.GetProperty(rItem.Key, rBindFlags);
                    if (rPropInfo != null)
                    {
                        rMemberType = rPropInfo.PropertyType;
                        object rValueObj = rItem.Value.ToObject(rMemberType);
                        rPropInfo.SetValue(rObject, rValueObj, null);
                        continue;
                    }
                }
                return rObject;
            }
            return null;
        }

        public override bool TryGetValue(string key, out JsonNode value)
        {
            return dict.TryGetValue(key, out value);
        }

        /// <summary>
        /// 转化Key
        /// </summary>
        private object GetKey_ByString(Type rKeyType, string rKeyStr)
        {
            object rKey = rKeyStr;
            if (rKeyType == typeof(int))
            {
                int rIntKey = 0;
                int.TryParse(rKeyStr, out rIntKey);
                rKey = rIntKey;
            }
            else if (rKeyType == typeof(long))
            {
                long rLongKey = 0;
                long.TryParse(rKeyStr, out rLongKey);
                rKey = rLongKey;
            }
            return rKey;
        }

        public override object ToDict(Type rDictType, Type rKeyType, Type rValueType)
        {
            if (rDictType.IsGenericType && typeof(IDictionary).IsAssignableFrom(rDictType.GetGenericTypeDefinition()))
            {
                // 特殊处理IDictionary<,>类型
                IDictionary rObject = (IDictionary)ReflectionAssist.CreateInstance(rDictType, BindingFlags.Default);
                foreach (var rItem in this.dict)
                {
                    object rKey = GetKey_ByString(rKeyType, rItem.Key);
                    object rValue = rItem.Value.ToObject(rValueType);
                    rObject.Add(rKey, rValue);
                }
                return rObject;
            }
            else if (rDictType.IsGenericType && typeof(IDict).IsAssignableFrom(rDictType.GetGenericTypeDefinition()))
            {
                // 特殊处理IDict<,>的类型
                IDict rObject = (IDict)ReflectionAssist.CreateInstance(rDictType, BindingFlags.Default);
                foreach (var rItem in this.dict)
                {
                    object rKey = GetKey_ByString(rKeyType, rItem.Key);
                    object rValue = rItem.Value.ToObject(rValueType);
                    rObject.AddObject(rKey, rValue);
                }
                return rObject;
            }
            return null;
        }
    }

    public class JsonData : JsonNode
    {
        private string  value;

        public JsonData(string v)
        {
            value = v;
        }

        public JsonData(float v)
        {
            AsFloat = v;
        }

        public JsonData(double v)
        {
            AsDouble = v;
        }

        public JsonData(int v)
        {
            AsInt = v;
        }

        public JsonData(long v)
        {
            AsLong = v;
        }

        public JsonData(bool v)
        {
            AsBool = v;
        }

        public override string Value
        {
            get { return this.value;  }
            set { this.value = value; }
        }

        public override string ToString()
        {
            return "\"" + value.ToString() + "\"";
        }

        public override List<string> Keys
        {
            get
            {
                var rKeys = new List<string>();
                rKeys.Add(this.Key);
                return rKeys;
            }
        }

        public override bool ContainsKey(string rKey)
        {
            return rKey.Equals(this.Key);
        }

        public override object ToObject(Type rType)
        {
            if (rType.IsPrimitive)
            {
                if (rType == typeof(int) || rType == typeof(uint))
                {
                    return CastInt(this.value);
                }
                else if (rType == typeof(long))
                {
                    return CastLong(this.value);
                }
                else if (rType == typeof(float))
                {
                    return CastFloat(this.value);
                }
                else if (rType == typeof(double))
                {
                    return CastDouble(this.value);
                }
                else if (rType == typeof(bool))
                {
                    return CastBool(this.value);
                }
            }
            else if (rType.IsEnum)
            {
                return CastEnum(rType, this.value);
            }
            else if (rType == typeof(string))
            {
                return this.value;
            }
            Model.Log.Error($"{this.value}不是基础类型，不能解析成为JsonData !");
            return this.value.Trim('"');
        }
    }
}
