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

namespace KnightHotfixModule.Test
{
    public class Class1 : MonoBehaviourProxy
    {
        public override void SetObjects(List<UnityEngine.Object> rObjs)
        {
            base.SetObjects(rObjs);
        }

        public override void Start()
        {
            Debug.LogError("Start...");
            Debug.LogError(this.Objects[0].name);

            //GameObject rGo = this.Objects[0] as GameObject;
            //var rHotfixMBTest = rGo.GetComponent<HotfixMBTest>();
            //rHotfixMBTest.Test1();
            
            var rHotfixMBTest = this.Objects[0] as HotfixMBTest;
            rHotfixMBTest.Test1();
        }
    }
}
