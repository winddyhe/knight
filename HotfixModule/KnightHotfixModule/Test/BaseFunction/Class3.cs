using System;
using System.Collections.Generic;
using UnityEngine;
using WindHotfix.Test1;

namespace WindHotfix.Test1
{
    public class TClass3<T> where T : class
    {
        public virtual void Test()
        {
            Debug.LogError("TClass3 test..");
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
