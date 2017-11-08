using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Hotfix
{
    public class HotfixApp
    {
        public virtual async Task Load(string rABPath, string rHotfixModuleName)
        {
        }

        public virtual void InitApp(string rDLLPath, string rPDBPath)
        {
        }

        public virtual void InitApp(byte[] rDLLBytes, byte[] rPDBBytes)
        {
        }

        public virtual HotfixObject Instantiate(string rTypeName, params object[] rArgs)
        {
            return null;
        }

        public virtual T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            return default(T);
        }

        public virtual object Invoke(HotfixObject rHotfixObj, string rMethodName, params object[] rArgs)
        {
            return null;
        }

        public virtual object InvokeParent(HotfixObject rHotfixObj, string rParentType, string rMethodName, params object[] rArgs)
        {
            return null;
        }

        public virtual object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        { 
            return null;
        }
    }
}
