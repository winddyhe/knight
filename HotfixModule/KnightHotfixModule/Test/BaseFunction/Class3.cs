using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Test1;
using Core;
using Core.WindJson;

namespace WindHotfix.Test1
{
    public class TClass3<T> where T : class
    {
        public class A1
        {
            public double C;
        }

        public virtual void Test()
        {
            Debug.LogError("TClass3 test..");

            Dict<int, int> a = new Dict<int, int>();
            for (int i = 0; i < 10; i++)
            {
                a.Add(i, i);
            }
            foreach (var rItem in a)
            {
                Debug.LogError(rItem.Key + ", " + rItem.Value);
            }

            //JsonNode rNode = JsonParser.Parse("{\"C\":2.345 }");
            //Debug.LogError(rNode.ToString());
            //var A1 = rNode.ToObject<A1>();
            //Debug.LogError(A1.C);
        }
    }
}

namespace WindHotfix.Test
{
    public class Class3 : TClass3<Class3>
    {
        //public override void Test()
        //{
        //    base.Test();
        //}

        public void Test1()
        {
            Class3 c = new Class3();
            c.Test();
        }
    }
}
