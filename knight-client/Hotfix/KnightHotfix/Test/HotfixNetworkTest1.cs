using Knight.Core;
using Knight.Framework.Net;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using Knight.Hotfix.Core;
using Knight.Framework;

namespace Game.Test
{
    public class HotfixNetworkTest2 : HotfixKnightObject
    {
        [HotfixMessageHandler(HotfixNetworkMessageOpcode.G2C_Test1)]
        public void MessageHandlerTest1(EventArg rEventArg)
        {
            var rTestMsg = rEventArg.Get<G2C_TestHotfixMessage>(0);
            Debug.LogError("MessageHandlerTest1: " + rTestMsg.Info);
        }

        [HotfixEvent(GameEvent.EVENT_TEST_1)]
        public void EventHandlerTest1(EventArg rEventArg)
        {
            var rTestMsg = rEventArg.Get<string>(0);
            Debug.LogError("EventHandlerTest1: " + rTestMsg);
        }
    }

    public class HotfixNetworkTest1
    {
        private static long LoginKey = 0;

        public static async Task<string> Test1(NetworkSession rSession)
        {
            var rTest2 = new HotfixNetworkTest2();
            await rTest2.Initialize();

            EventManager.Instance.Distribute(GameEvent.EVENT_TEST_1, "hahahahahaha...");

            // 设置热更新端的Session
            HotfixNetworkClient.Instance.Session = rSession;

            // Login
            R2C_Login r2CLogin = (R2C_Login)await HotfixNetworkClient.Instance.Call(new C2R_Login() { Account = "WinddyHe", Password = "111111" });
            Debug.LogError("Address: " + r2CLogin.Address);
            LoginKey = r2CLogin.Key;

            return r2CLogin.Address;
        }

        public static async Task Test2(NetworkSession rSession)
        {
            // 设置热更新端的Session
            HotfixNetworkClient.Instance.Session = rSession;
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await HotfixNetworkClient.Instance.Call(new C2G_LoginGate() { Key = LoginKey });

            Debug.LogError("PlayerID: " + g2CLoginGate.PlayerId);
        }
    }

    public class BagItem
    {
        public int  id;
        public int  Count;
    }
}
