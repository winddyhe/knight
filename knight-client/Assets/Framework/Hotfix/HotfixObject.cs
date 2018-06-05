//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
namespace Knight.Framework.Hotfix
{
    public class HotfixObject
    {
        public string           TypeName;
        public HotfixApp        App;
        public object           Object;

        public HotfixObject(HotfixApp rApp, string rTypeName)
        {
            this.App = rApp;
            this.TypeName = rTypeName;
        }

        public object Invoke(string rMethodName, params object[] rArgs)
        {
            if (this.App == null || this.Object == null) return null;
            return this.App.Invoke(this, rMethodName, rArgs);
        }

        public object InvokeParent(string rParentType, string rMethodName, params object[] rArgs)
        {
            if (this.App == null || this.Object == null) return null;
            return this.App.InvokeParent(this, rParentType, rMethodName, rArgs);
        }

        public object InvokeStatic(string rMethodName, params object[] rArgs)
        {
            if (this.App == null) return null;
            return this.App.InvokeStatic(this.TypeName, rMethodName, null, rArgs);
        }
    }
}
