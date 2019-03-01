using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Framework.Net;
using System.Net;
using Knight.Core;
using Knight.Framework;
using Knight.Framework.Hotfix;
using System.Threading.Tasks;

namespace Game.Test
{
    public class MessageTestObject : KnightObject
    {
        public void OnHandlerNetTest1(EventArg rEventArg)
        {
            Debug.LogError("KnightObject Test...");
        }
    }

    public class NetworkTest1 : MonoBehaviour
    {
        private MessageTestObject   mNetTest;

        private async void Start()
        {
            HotfixManager.Instance.Initialize(); 
            CoroutineManager.Instance.Initialize();
            EventManager.Instance.Initialize();

            mNetTest = new MessageTestObject();
            await mNetTest.Initialize();

            string rDLLPath = "Assets/StreamingAssets/Temp/Libs/KnightHotfix.dll";
            string rPDBPath = "Assets/StreamingAssets/Temp/Libs/KnightHotfix.pdb";
            HotfixManager.Instance.InitApp(rDLLPath, rPDBPath);

            NetworkClient.Instance.Initialize(NetworkProtocol.TCP);

            // Login Conn
            IPEndPoint rIPEndPoint = UtilTool.ToIPEndPoint("127.0.0.1", 10002);
            var rConnSession = NetworkClient.Instance.Create(rIPEndPoint);
            var rAddress = await (HotfixManager.Instance.InvokeStatic("Game.Test.HotfixNetworkTest1", "Test1", rConnSession) as Task<string>);
            rConnSession.Dispose();

            // LoginGate
            var rGateEndPoint = UtilTool.ToIPEndPoint(rAddress);
            var rGateSession = NetworkClient.Instance.Create(rGateEndPoint);
            await (HotfixManager.Instance.InvokeStatic("Game.Test.HotfixNetworkTest1", "Test2", rGateSession) as Task);
        }

        private void OnDestroy()
        {
            NetworkClient.Instance.Dispose();
            if (mNetTest != null) mNetTest.Dispose();
        }

        private void Update()
        {
            NetworkClient.Instance.Update();
        }
    }
}