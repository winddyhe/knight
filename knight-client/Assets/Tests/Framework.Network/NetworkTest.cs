using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Framework.Net;
using System.Net;
using Knight.Core;
using Knight.Framework.Game;
using Knight.Framework;

namespace Game.Test
{
    public class MessageTestObject : KnightObject
    {
        [MessageHandler(NetworkMessageOpcode.G2C_Test1)]
        public void OnHandlerNetTest1(EventArg rEventArg)
        {
            Debug.LogError("KnightObject Test...");

            var rMsg = rEventArg.Get<G2C_TestHotfixMessage>(0);
            if (rMsg == null) return;
            Debug.LogError(rMsg.Info);
        }
    }

    public class NetworkTest : MonoBehaviour
    {
        private NetworkClient       mClient;
        private NetworkSession      mConnSession;
        private NetworkSession      mGateSession;

        private MessageTestObject   mNetTest;

        private async void Start()
        {
            EventManager.Instance.Initialize();

            mNetTest = new MessageTestObject();
            await mNetTest.Initialize();

            mClient = new NetworkClient();
            IPEndPoint rIPEndPoint = UtilTool.ToIPEndPoint("127.0.0.1", 10002);
            mClient.Initialize(NetworkProtocol.TCP);
            mConnSession = mClient.Create(rIPEndPoint);
            
            // Login
            R2C_Login r2CLogin = (R2C_Login)await this.mConnSession.Call(new C2R_Login() { Account = "WinddyHe", Password = "111111" });
            this.mConnSession.Dispose();
            Debug.LogError(r2CLogin.Address);

            // LoginGate
            var connetEndPoint = UtilTool.ToIPEndPoint(r2CLogin.Address);
            this.mGateSession = mClient.Create(connetEndPoint);
            G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await this.mGateSession.Call(new C2G_LoginGate() { Key = r2CLogin.Key });
            Debug.LogError(g2CLoginGate.PlayerId);
        }

        private void OnDestroy()
        {
            mClient.Dispose();
            mNetTest.Dispose();
        }

        private void Update()
        {
            if (mClient == null) return;
            this.mClient.Update();
        }
    }
}