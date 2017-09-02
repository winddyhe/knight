//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace WindHotfix.Core
{
    public class THotfixMB<T> : HotfixMB where T : class
    {
        public List<UnityObject>        Objects;
        public List<BaseDataObject>     BaseDatas;
        
        public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.Objects   = rObjs;
            this.BaseDatas = rBaseDatas;
            
            this.OnInitialize();
        }

        public virtual void OnInitialize()
        {
        }
        
        /// <summary>
        /// @TODO: 这样子做可能有风险，无法执行到OnDestroy导致mEventHandler的引用计数不对
        ///        等框架完善之后再做改进
        /// </summary>
        public void Destroy()
        {
            this.GameObject = null;

            this.OnDestroy();
        }
        
        public object GetData(string rName)
        {
            if (this.BaseDatas == null) return null;
            var rBaseDataObj = this.BaseDatas.Find((rItem) => { return rItem.Name.Equals(rName); });
            if (rBaseDataObj == null) return null;
            return rBaseDataObj.Object;
        }
    }
}
