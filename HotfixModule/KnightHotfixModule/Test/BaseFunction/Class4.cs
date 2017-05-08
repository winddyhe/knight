using System;
using System.Collections.Generic;
using WindHotfix.Core;
using UnityEngine;
using Framework.Hotfix;

namespace WindHotfix.Test
{
    public class Class4 : THotfixMB<Class4>
    {
        public override void Start()
        {
            var rMBC = this.Objects[0].Object as HotfixMBContainer;
            Debug.LogError(rMBC.MBHotfixObject as Class5);

            var rClass5 = rMBC.MBHotfixObject as Class5;
            Debug.LogError(rClass5.name);
        }
    }
}
