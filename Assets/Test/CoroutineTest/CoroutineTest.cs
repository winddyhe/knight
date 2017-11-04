//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using System.Threading.Tasks;

namespace Test
{
    public class CoroutineTest : MonoBehaviour
    {
        private async void Start()
        {
            CoroutineManager.Instance.Initialize();
            await Start_Async();
        }

        private async Task Start_Async()
        {
            CoroutineLoadTest rTest = new CoroutineLoadTest();
            Debug.Log(await rTest.Loading("Test1"));
            Debug.Log(await rTest.Loading("Test2"));

            Debug.Log(await rTest.GetValueExampleAsync());

            Debug.Log("Done.");
        }
    }
}
