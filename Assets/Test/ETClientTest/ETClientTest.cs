using UnityEngine;
using Model;
using System;
using Framework.Hotfix;
using System.IO;
using Core;
using System.Threading.Tasks;
using Framework.Network;

namespace Test
{
    public class ETClientTest : MonoBehaviour
    {
        Session gateSession;

        async void Start()
        {
            HotfixManager.Instance.Initialize();
            CoroutineManager.Instance.Initialize();

            string rDLLPath = "Assets/StreamingAssets/Temp/Libs/KnightHotfixModule.dll";
            string rPDBPath = "Assets/StreamingAssets/Temp/Libs/KnightHotfixModule.pdb";
            HotfixManager.Instance.InitApp(rDLLPath, rPDBPath);

            ETClient.Instance.Initialize(NetworkProtocol.TCP);

            Session session = null;
            try
            {
                session = ETClient.Instance.Create("127.0.0.1:10002");
                string rAddress = await (HotfixManager.Instance.InvokeStatic("KnightHotfixModule.Test.Net.ETNetworkTest", "Test1", session) as Task<string>);

                gateSession = ETClient.Instance.Create(rAddress);
                HotfixManager.Instance.InvokeStatic("KnightHotfixModule.Test.Net.ETNetworkTest", "Test2", gateSession);

                Debug.LogError("Test completed..");
            }
            catch(Exception e)
            {
                Debug.LogError(e.ToString());
            }
            finally
            {
                session?.Dispose();
            }
        }

        void Update()
        {
            ETClient.Instance.Update();
        }

        void OnDestroy()
        {
            gateSession.Dispose();
        }
    }
}
