//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Core;

namespace Test
{
    /// <summary>
    /// 使用的部分
    /// </summary>
    public class LoaderRequest
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

        private async Task<LoaderRequest> Load(string rPath)
        {
            LoaderRequest rRequest = new LoaderRequest(rPath);
            await new WaitForSeconds(1.0f);
            rRequest.obj = new GameObject(rRequest.path);
            Debug.LogFormat("Create GameObject: {0}", rRequest.path);
            return rRequest;
        }

        public async Task<string> Loading(string rPath)
        {
            return (await Load(rPath)).path;
        }

        public async Task<string> GetValueExampleAsync()
        {
            await new WaitForSeconds(1.0f);
            return "asdf";
        }
    }
}
