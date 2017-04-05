//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;

namespace Test
{
    /// <summary>
    /// 使用的部分
    /// </summary>
    public class LoaderRequest : CoroutineRequest<LoaderRequest>
    {
        public Object obj;
        public string path;

        public LoaderRequest(string rPath)
        {
            this.path = rPath;
        }
    }

    public class CoroutineLoadTest
    {
        public LoaderRequest mRequest;

        private LoaderRequest Load_Async(string rPath)
        {
            LoaderRequest rRequest = new LoaderRequest(rPath);
            mRequest = rRequest;
            rRequest.Start(Load(rRequest));
            return rRequest;
        }

        private IEnumerator Load(LoaderRequest rRequest)
        {
            yield return new WaitForSeconds(1.0f);
            rRequest.obj = new GameObject(rRequest.path);
            Debug.LogFormat("Create GameObject: {0}", rRequest.path);
        }

        public IEnumerator Loading(string rPath)
        {
            LoaderRequest rRequest = Load_Async(rPath);
            yield return rRequest;
        }
    }

    public class CoroutineTest : MonoBehaviour
    {
        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();
            yield return CoroutineManager.Instance.Start(Start_Async());
        }

        IEnumerator Start_Async()
        {
            CoroutineLoadTest rTest = new CoroutineLoadTest();
            yield return CoroutineManager.Instance.Start(rTest.Loading("Test1"));
            yield return CoroutineManager.Instance.Start(rTest.Loading("Test2"));
            Debug.Log("Done.");
        }
    }
}
