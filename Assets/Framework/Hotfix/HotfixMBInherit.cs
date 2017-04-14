//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Framework.Hotfix
{
    public class HotfixMBInherit
    {
        public List<UnityObject>        Objects;
        public List<BaseDataObject>     BaseDatas;
        
        public HotfixMBInherit()
        {
        }

        public virtual void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.Objects = rObjs;
            this.BaseDatas = rBaseDatas;
        }
    }
}
