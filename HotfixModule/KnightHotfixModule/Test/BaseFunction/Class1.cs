//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Test;
using Framework.Hotfix;
using Core;
using WindHotfix.Core;

namespace WindHotfix.Test
{
    public class Class1 : THotfixMonoBehaviour<Class1>
    {
        public override void Start()
        {
            Debug.LogError("Start...");
            Debug.LogError(this.Objects[0].Object.name);

            //GameObject rGo = this.Objects[0] as GameObject;
            //var rHotfixMBTest = rGo.GetComponent<HotfixMBTest>();
            //rHotfixMBTest.Test1();
            
            var rHotfixMBTest = this.Objects[0].Object as HotfixMBTest;
            rHotfixMBTest.Test1();

            Dictionary<int, int> rDictTest = new Dictionary<int, int>();
            for (int i = 0; i < 10; i++)
            {
                rDictTest.Add(i, i);
            }
            foreach (var rItem in rDictTest)
            {
                Debug.LogError(rItem.Key + ", " + rItem.Value);
            }
        }
    }
}
