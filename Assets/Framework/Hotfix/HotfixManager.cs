//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;
using System.IO;
using UnityEngine;

namespace Framework.Hotfix
{
    public class HotfixManager : TSingleton<HotfixManager>
    {
        public static string    IsHotfixDebugModeKey = "HofixManager_IsHofixDebugMode";
        
        private HotfixApp       mApp;
        
        private HotfixManager()
        {
        }

        public void Initialize()
        {
            Debug.LogFormat("IsHotfixDebugModeKey: {0}", this.IsHotfixDebugMode());
            
            if (IsHotfixDebugMode())
                mApp = new HotfixApp_Reflect();
            else
                mApp = new HotfixApp_ILRT();
        }

        public void InitApp(byte[] rDLLBytes, byte[] rPDBBytes)
        {
            if (mApp == null) return;
            mApp.InitApp(rDLLBytes, rPDBBytes);
        }

        public void InitApp(string rDLLPath, string rPDBPath)
        {
            if (mApp == null) return;
            mApp.InitApp(rDLLPath, rPDBPath);
        }

        public async Task Load(string rABPath, string rHotfixModuleName)
        {
            if (mApp == null) return;
            await mApp.Load(rABPath, rHotfixModuleName);
        }

        public HotfixObject Instantiate(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.Instantiate(rTypeName, rArgs);
        }

        public T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            if (mApp == null) return default(T);
            return mApp.Instantiate<T>(rTypeName, rArgs);
        }

        public object Invoke(HotfixObject rHotfixObj, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.Invoke(rHotfixObj, rMethodName, rArgs);
        }

        public object InvokeParent(HotfixObject rHotfixObj, string rParentType, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.InvokeParent(rHotfixObj, rParentType, rMethodName, rArgs);
        }

        public object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            if (mApp == null) return null;
            return mApp.InvokeStatic(rTypeName, rMethodName, rArgs);
        }

        public bool IsHotfixDebugMode()
        {
#if UNITY_EDITOR
            bool bIsHotfixDebugMode = false;
            bIsHotfixDebugMode = UnityEditor.EditorPrefs.GetBool(HotfixManager.IsHotfixDebugModeKey);
            return bIsHotfixDebugMode;
#else
    #if HOTFIX_REFLECT_USE
            return true;
    #else
            return false;
    #endif
#endif
        }

        public Type[] GetTypes()
        {
            if (mApp == null) return null;
            return mApp.GetTypes();
        }
    }
}
