//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.IO;
using System.Reflection;
using Core;

namespace Framework.Hotfix
{
    public class HotfixApp_Reflect : HotfixApp
    {
        private Assembly    mApp;

        public override void InitApp(byte[] rDLLBytes, byte[] rPDBBytes)
        {
            mApp = Assembly.Load(rDLLBytes, rPDBBytes);
        }

        public override HotfixObject Instantiate(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return null;
            var rObject = new HotfixObject(this, rTypeName);
            rObject.Object = Activator.CreateInstance(mApp.GetType(rTypeName), rArgs);
            return rObject;
        }

        public override T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return default(T);
            return (T)Activator.CreateInstance(mApp.GetType(rTypeName), rArgs);
        }

        public override object Invoke(HotfixObject rHotfixObj, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            Type rObjType = mApp.GetType(rHotfixObj.TypeName);
            return ReflectionAssist.MethodMember(rHotfixObj.Object, rMethodName, ReflectionAssist.flags_method_inst, rArgs);
        }

        public override object InvokeParent(HotfixObject rHotfixObj, string rParentType, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            Type rObjType = mApp.GetType(rParentType);
            return ReflectionAssist.MethodMember(rHotfixObj.Object, rMethodName, ReflectionAssist.flags_method_inst, rArgs);
        }

        public override object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            Type rObjType = mApp.GetType(rTypeName);
            return rObjType.InvokeMember(rMethodName, ReflectionAssist.flags_method, null, null, rArgs);
        }
    }
}
