using Knight.Framework.Hotfix;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public static class HotfixRegister
    {
        public static void Register()
        {
            HotfixApp_ILRT.OnHotfixRegisterFunc = (rApp) => 
            {
                rApp.DelegateManager.RegisterMethodDelegate<Knight.Framework.Net.AChannel, System.Int32>();
            };
        }
    }
}
