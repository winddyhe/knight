using System;
using System.Collections.Generic;
using UnityEngine;

namespace KnightHotfixModule
{
    public class Class1
    {
        private int n;
        public Class1(int i)
        {
            n = i;
        }

        public void Test1()
        {
            Debug.LogError("Hello hotfix..." + n);
        }

        public static void Test2()
        {
            Debug.LogError("我是静态函数Test2...");
        }
    }
}
