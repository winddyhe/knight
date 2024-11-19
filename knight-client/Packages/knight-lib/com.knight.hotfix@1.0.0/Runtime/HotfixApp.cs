using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Knight.Framework.Hotfix
{
    public abstract class HotfixApp
    {
#pragma warning disable 1998
        public virtual async Task Load(string rABPath, string rHotfixModuleName)
        {
        }
        public virtual void LoadDLL(string rHotfixModuleName)
        {
        }
#pragma warning restore 1998

        public virtual object Instantiate(string rTypeName, params object[] rArgs)
        {
            return null;
        }

        public virtual T Instantiate<T>(string rTypeName, params object[] rArgs)
        {
            return default(T);
        }

        public virtual object Invoke(object rObj, string rTypeName, string rMethodName, params object[] rArgs)
        {
            return null;
        }

        public virtual object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            return null;
        }

        public virtual Type[] GetTypes()
        {
            return null;
        }

        public virtual void Dispose()
        {
        }
    }
}
