using System;
using System.Collections.Generic;
using WindHotfix.Core;

namespace WindHotfix.Test
{
    public class Class5 : THotfixMB<Class5>
    {
        public string name = "Class5";

        public override void OnInitialize()
        {
            this.name = "OnInitialize Class5";
        }
    }
}
