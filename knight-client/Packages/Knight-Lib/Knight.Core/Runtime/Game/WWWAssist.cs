//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityFx.Async;

namespace Knight.Core
{
    /// <summary>
    /// WWW的辅助类
    /// </summary> 
    public class WWWAssist
    {
        public class LoaderRequest : AsyncRequest<LoaderRequest>
        {
            public string       Text;
            public byte[]       Bytes;
            public string       Url;

            public LoaderRequest(string rPath)
            {
                this.Url = rPath;
            }
        }

        public static WWW Load(string rURL)
        {
            WWW www = new WWW(rURL);
            return www;
        }

        public static void Destroy(ref WWW www)
        {
            if (www == null) return;

            www.Dispose();
            www = null;
        }

        public static void Dispose(ref WWW www, bool isUnloadAllLoadedObjects = false)
        {
            if (www == null) return;
            
            if (string.IsNullOrEmpty(www.error) && www.assetBundle != null)
                www.assetBundle.Unload(isUnloadAllLoadedObjects);

            www.Dispose();
            www = null;
        }

        public static IAsyncOperation<LoaderRequest> LoadFile(string rURL)
        {
            var rRequest = new LoaderRequest(rURL);
            return rRequest.Start(LoadFile(rRequest));
        }

        public static IEnumerator LoadFile(LoaderRequest rRequest)
        {
            WWW www = WWWAssist.Load(rRequest.Url);
            yield return www;

            rRequest.Text = www.text;
            rRequest.Bytes = www.bytes;

            rRequest.SetResult(rRequest);

            WWWAssist.Destroy(ref www);
        }
    }
}
