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
        public AppDomain        App;

        public HotfixObject(AppDomain rApp, string rTypeName)
        {
            this.App = rApp;
            this.TypeName = rTypeName;
        }

        public void CreateInstance(params object[] rArgs)
        {
            if (this.App == null) return;
            this.ILObject = this.App.Instantiate(this.TypeName, rArgs);
        }

        public object InvokeInstance(string rMethodName, params object[] rArgs)
        {
            if (this.App == null || this.ILObject == null) return null;
            return this.App.Invoke(this.TypeName, rMethodName, this.ILObject, rArgs);
        }

        public object InvokeStatic(string rMethodName, params object[] rArgs)
        {
            if (this.App == null) return null;
            return this.App.Invoke(this.TypeName, rMethodName, null, rArgs);
        }

        public static object InvokeStatic(AppDomain rApp, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return rApp.Invoke(rTypeName, rMethodName, null, rArgs);
        }
    }
}
