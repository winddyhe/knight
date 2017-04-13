//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Enviorment;

namespace Framework.Hotfix
{
    public class HotfixObject
    {
        public string           TypeName;

        public ILTypeInstance   ILObject;
        public HotfixApp        App;

        public HotfixObject(HotfixApp rApp, string rTypeName)
        {
            this.App = rApp;
            this.TypeName = rTypeName;
        }
        
        public object Invoke(string rMethodName, params object[] rArgs)
        {
            if (this.App == null || this.ILObject == null) return null;
            return this.App.Invoke(this, rMethodName, rArgs);
        }

        public object InvokeParent(string rParentType, string rMethodName, params object[] rArgs)
        {
            if (this.App == null || this.ILObject == null) return null;
            return this.App.InvokeParent(this, rParentType, rMethodName, rArgs);
        }

        public object InvokeStatic(string rMethodName, params object[] rArgs)
        {
            if (this.App == null) return null;
            return this.App.InvokeStatic(this.TypeName, rMethodName, null, rArgs);
        }

        public static object InvokeStatic(HotfixApp rApp, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return rApp.InvokeStatic(rTypeName, rMethodName, null, rArgs);
        }
    }
}
