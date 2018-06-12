using Knight.Core;
using Knight.Framework.Net;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Hotfix.Core;

namespace Game.Test
{
    public class HotfixNetworkTest1
    {
        private static long LoginKey = 0;

        public static async Task<string> Test1(NetworkSession rSession)
        {
            // 设置热更新端的Session
            HotfixNetworkClient.Instance.Session = rSession;

            // Login
            R2C_Login r2CLogin = (R2C_Login)await HotfixNetworkClient.Instance.Call(new C2R_Login() { Account = "WinddyHe", Password = "111111" });
            Debug.LogError(r2CLogin.Address);
            LoginKey = r2CLogin.Key;

            return r2CLogin.Address;
        }

        public static async Task Test2(NetworkSession rSession)
        {
            // 设置热更新端的Session
            HotfixNetworkClient.Instance.Session = rSession;
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await HotfixNetworkClient.Instance.Call(new C2G_LoginGate() { Key = LoginKey });

            Debug.LogError(g2CLoginGate.PlayerId);
        }
    }
}
