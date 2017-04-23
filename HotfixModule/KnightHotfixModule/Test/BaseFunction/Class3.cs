using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Test1;
using Core;
using Core.WindJson;

namespace WindHotfix.Test1
{
    public class A1
    {
        public double C;

        public List<float> B;
    }

    public class TClass3<T> where T : class
    {
        public virtual void Test()
        {
            Debug.LogError("TClass3 test..");

            //Dict<int, int> a = new Dict<int, int>();
            //for (int i = 0; i < 10; i++)
            //{
            //    a.Add(i, i);
            //}
            //foreach (var rItem in a)
            //{
            //    Debug.LogError(rItem.Key + ", " + rItem.Value);
            //}

            //JsonNode rNode = JsonParser.Parse("{\"C\":2.345, \"B\":[2.1, 2.2] }");
            //Debug.LogError(rNode.ToString());
            //var A1 = (A1)JsonParser.ToObject(rNode, typeof(A1));
            //Debug.LogError(A1.B[0]);

            JsonNode rNode = JsonParser.Parse("[{\"C\":2.345, \"B\":[2.1, 2.2] }, {\"C\":2.346, \"B\":[2.2, 2.3] }]");
            Debug.LogError(rNode.ToString());
            var A1 = (List<A1>)JsonParser.ToObject(rNode, typeof(List<A1>));
            Debug.LogError(A1[1].B[0]);


            //Debug.LogError("2222");
            //var A1 = JsonMapper.ToObject<A1>(new JsonReader("{\"C\":2.345 }")) as A1;
            //Debug.LogError(A1.C);

            //Debug.LogError(typeof(List<A1>).FullName);
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
