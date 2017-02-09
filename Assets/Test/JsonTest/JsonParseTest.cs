//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Core;
using Core.WindJson;

namespace Test
{
    public class JsonParseTest : MonoBehaviour
    {
        public class A
        {
            public int B1 { get; set; }
            public long B2 { get; set; }
            public A1[] A1 { get; set; }

            public List<int> D;

            public Dictionary<int, A1[]> E;

            public Dict<int, Dict<int, A1>> F;
        }

        public class A1
        {
            public double C;
        }

        public class C
        {
            public int[] C1;
        }

        void OnEnable()
        {
            //Test1();

            Test2();
        }

        private void Test2()
        {
            string rText = File.ReadAllText("Assets/Test/JsonTest/Test1.txt");
            JsonParser rJsonParser = new JsonParser(rText);
            Debug.Log(rJsonParser.PretreatmentProc());

            JsonNode rNode = rJsonParser.Parser();
            Debug.Log(rNode.ToString());

            //JsonNode rJsonNode = JsonParser.ToJsonNode();
            //Debug.LogError(rJsonNode.ToString());

            //JsonNode rRootNode = new JsonClass();
            //rRootNode["name"] = new JsonData("Winddy");
            //rRootNode["age"] = new JsonData(12);
            //JsonNode rArray = new JsonArray();
            //rArray.Add(new JsonData("book1"));
            //rArray.Add(new JsonData("book2"));
            //rArray.Add(new JsonData("book3"));
            //rRootNode["books"] = rArray;
            //Debug.Log(rRootNode.ToString());
        }

        void Test1()
        {
            JsonParser rJsonParser = new JsonParser(File.ReadAllText("Assets/Test/JsonTest/Test1.txt"));
            Debug.Log(rJsonParser.PretreatmentProc());

            JsonNode rNode = rJsonParser.Parser();
            Debug.LogError(rNode.ToString());

            A a = rNode.ToObject(typeof(A)) as A;
            Debug.LogError(string.Format("{0}, {1}, {2}", a.B1, a.B2, a.A1[1].C));
            for (int i = 0; i < a.D.Count; i++)
            {
                Debug.LogError(a.D[i]);
            }
            foreach (var item in a.E)
            {
                Debug.LogError(string.Format("E: ({0}, {1})", item.Key, item.Value[1].C));
            }

            foreach (var item in a.F)
            {
                foreach (var secondItem in item.Value)
                {
                    Debug.LogError(string.Format("F: ({0}, {1})", item.Key, secondItem.Value.C));
                }
            }

            JsonNode rJsonNode = JsonParser.ToJsonNode(a);
            Debug.LogError(rJsonNode.ToString());

            //List<A> rLists =  rJsonNode.ToList<A>();
            //A[] rArrays = rJsonNode.ToArray<A>();
            //Dict<string, A> rDict1 = rJsonNode.ToDict<string, A>();
            //Dictionary<string, A> rDict2 = rJsonNode.ToDictionary<string, A>();
        }
    }
}

