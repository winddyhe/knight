using Knight.Hotfix.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Test
{
    public class B
    {
        public float C;
    }
    
    public class A
    {
        public int      B1;
        public long     B2;
        public List<B>  A1;
        public int[]    D;

        public Dictionary<int, List<B>> E;
        public Dictionary<int, Dictionary<int, B>> F;
    }

    public class HotfixJsonTest1
    {
        public static void Test(string rJsonText)
        {
            //Debug.LogError(rJsonText);
            //var rJsonNode = HotfixJsonParser.Parse(rJsonText);
            //Debug.LogError("xxxx1");
            //var rObj = HotfixJsonParser.ToObject<A>(rJsonNode);
            //Debug.LogError("xxxx2");
            //Debug.LogError(rObj.B1);
            //foreach (var rItem in rObj.E)
            //{
            //    foreach (var rItem1 in rItem.Value)
            //    {
            //        Debug.LogError(rItem1.Value.C);
            //    }
            //}
            //rJsonNode = HotfixJsonParser.ToJsonNode(rObj);
            //Debug.LogError(rJsonNode.ToString());

            var rA = new A()
            {
                B1 = 199,
                B2 = 20000,
                A1 = new List<B> { new B { C = 0.1003f, }, new B { C = 1.332f } },
                D = new int[] { 2324, 42, 242 },
                E = new Dictionary<int, List<B>>() {
                    { 1, new List<B> { new B { C = 1.0232f }, new B { C = 243243.32f } } },
                    { 2, new List<B> { new B { C = 1.0232f }, new B { C = 243243.32f } } } },
                F = new Dictionary<int, Dictionary<int, B>>()
                {
                    { 323, new Dictionary<int, B>(){ {23, new B { C = 3213.23f } } } },
                    { 234, new Dictionary<int, B>(){ {23, new B { C = 3213.23f } } } },
                }
            };
            var rJsonNode = HotfixJsonParser.ToJsonNode(rA);
            Debug.LogError(rJsonNode.ToString());

            var rNewA = HotfixJsonParser.ToObject<A>(rJsonNode);
            Debug.LogError(rNewA.B1);
            foreach (var rItem in rNewA.A1)
            {
                Debug.LogError(rItem.C);
                //foreach (var rItem1 in rItem.Value)
                //{
                //    Debug.LogError(rItem1.C);
                //}
            }
        }
    }
}



