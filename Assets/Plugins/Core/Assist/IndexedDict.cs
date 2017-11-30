using System;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class IndexedDict<TKey, TValue>
    {
        private Dict<TKey, TValue>  mDictionary;
        private List<TKey>          mKeyList;

        public IndexedDict()
        {
            mDictionary = new Dict<TKey, TValue>();
            mKeyList = new List<TKey>();
        }

        public void Add(TKey rKey, TValue rValue)
        {
            mDictionary.Add(rKey, rValue);
            mKeyList.Add(rKey);
        }

        public int Count
        {
            get { return mDictionary.Count;  }
        }

        public TValue this[TKey rKey]
        {
            get { return mDictionary[rKey];  }
            set { mDictionary[rKey] = value; }
        }

        public Dict<TKey, TValue> Dict
        {
            get { return mDictionary;        }
        }

        public List<TKey> Keys
        {
            get { return mKeyList;           }
        }
    }

    public static class IndexedDictExpand
    {
        public static bool TryGetValue<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict, TKey rKey, out TValue rValue)
        {
            return rIndexedDict.Dict.TryGetValue(rKey, out rValue);
        }
        
        public static TKey LastKey<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            return rIndexedDict.Dict.LastKey();
        }

        public static TValue LastValue<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            return rIndexedDict.Dict.LastValue();
        }

        public static TKey FirstKey<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            return rIndexedDict.Dict.FirstKey();
        }

        public static TValue FirstValue<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            return rIndexedDict.Dict.FirstValue();
        }

        public static CKeyValuePair<TKey, TValue> First<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            if (rIndexedDict.Count == 0) return null;
            return new CKeyValuePair<TKey, TValue>(rIndexedDict.FirstKey(), rIndexedDict.FirstValue());
        }

        public static CKeyValuePair<TKey, TValue> Last<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            if (rIndexedDict.Count == 0) return null;

            return new CKeyValuePair<TKey, TValue>(rIndexedDict.LastKey(), rIndexedDict.LastValue());
        }

        public static void Clear<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict)
        {
            rIndexedDict.Dict.Clear();
            rIndexedDict.Keys.Clear();
        }

        public static bool Remove<TKey, TValue>(this IndexedDict<TKey, TValue> rIndexedDict, TKey key)
        {
            rIndexedDict.Keys.Remove(key);
            return rIndexedDict.Dict.Remove(key);
        }
    }
}
