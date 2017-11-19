using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindHotfix.Net;
using UnityEngine;
using Game.Knight;
using WindHotfix.Core;
using Model;
using Framework.Network;

namespace KnightHotfixModule.Test.Net
{
    public class ETNetworkTest
    {
        public static long      key = 0;
        public static string    address = "";

        public static async Task<string> Test1(Session rSession)
        {
            R2C_Login r2CLogin = await NetworkClient.Instance.Call<R2C_Login>(rSession, new C2R_Login() { Account = "Test1", Password = "111111" });
            Debug.LogError($"R2C_Login: {r2CLogin.Address}");
            address = r2CLogin.Address;
            key = r2CLogin.Key;
            return r2CLogin.Address;
        }

        public static async Task Test2(Session rGateSession)
        {
            G2C_LoginGate g2CLoginGate = await NetworkClient.Instance.Call<G2C_LoginGate>(rGateSession, new C2G_LoginGate() { Key = key });
            Debug.LogError("登陆gate成功!");
            Debug.LogError(g2CLoginGate.PlayerId.ToString());
        }
    }
}
