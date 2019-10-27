using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using Knight.Core;

namespace Game.Test
{
    public class DictTest
    {
        public class A
        {
            public int      A1;
            public long     A2;
            public string   A3;
        }

        [Test]
        public void Dict_Test1()
        {
            var rDict1 = new Dict<string, A>();
            rDict1.Add("xxx1", new A() { A1 = 1, A2 = 10000000000, A3 = "XXX1" });
            rDict1.Add("xxx2", new A() { A1 = 2, A2 = 20000000000, A3 = "XXX2" });
            rDict1.Add("xxx3", new A() { A1 = 3, A2 = 30000000000, A3 = "XXX3" });
            
            Assert.AreEqual(rDict1["xxx1"].A1, 1);
            Assert.AreEqual(rDict1["xxx2"].A2, 20000000000);
            Assert.AreEqual(rDict1["xxx3"].A3, "XXX3");

            //LogAssert.Expect(LogType.Log, "1, 10000000000, XXX1");
            //LogAssert.Expect(LogType.Log, "2, 20000000000, XXX2");
            //LogAssert.Expect(LogType.Log, "3, 30000000000, XXX3");

            foreach (var rPair in rDict1)
            {
                Debug.Log(rPair.Value.A1 + ", " + rPair.Value.A2 + ", " + rPair.Value.A3);
            }

            A rA1 = null;
            Assert.AreEqual(rDict1.TryGetValue("xxx1", out rA1), true);
            Assert.AreEqual(rA1.A3, "XXX1");

            A rA4 = null;
            Assert.AreEqual(rDict1.TryGetValue("xxx4", out rA4), false);

            Assert.AreEqual(rDict1.FirstKey(), "xxx1");
            Assert.AreEqual(rDict1.FirstValue().A3, "XXX1");

            Assert.AreEqual(rDict1.LastKey(), "xxx3");
            Assert.AreEqual(rDict1.LastValue().A2, 30000000000);

            Assert.AreEqual(rDict1.Count, 3);

            rDict1.Clear();
            Assert.AreEqual(rDict1.Count, 0);
        }
    }
}