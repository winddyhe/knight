using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Framework.Net;
using System.Net;
using Knight.Core;


namespace Game.Test
{
    public class NetworkTest : MonoBehaviour
    {
        private NetworkClient   mClient;
        private NetworkSession  mConnSession;
        private NetworkSession  mGateSession;

        private async void Start()
        {
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
        }

        private void Update()
        {
            if (mClient == null) return;
            this.mClient.Update();
        }
    }
}