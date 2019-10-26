//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Threading.Tasks;
using UnityFx.Async;
using System;

namespace Knight.Core
{
    public class WebRequestAssist
    {
        public class LoaderRequest : AsyncRequest<LoaderRequest>
        {
            public string               Text;
            public byte[]               Bytes;
            public bool                 IsDone;

            public string               Url;

            public LoaderRequest(string rURL)
            {
                this.IsDone = false;
                this.Url = rURL;
            }
        }

        public static IAsyncOperation<LoaderRequest> DownloadFile(string rURL, System.Action<float> rDownloadProgress = null)
        {
            var rRequest = new LoaderRequest(rURL);
            return rRequest.Start(DownloadFile(rRequest));
        }

        private static IEnumerator DownloadFile(LoaderRequest rRequest)
        {
            UnityWebRequest rWebRequest = UnityWebRequest.Get(rRequest.Url);
            yield return rWebRequest.SendWebRequest();

            if (rWebRequest.isNetworkError || !string.IsNullOrEmpty(rWebRequest.error))
            {
                Debug.Log(rWebRequest.error);
                rWebRequest.Dispose();
                rRequest.SetResult(rRequest);
                yield break;
            }

            var rDownloadHandler = rWebRequest.downloadHandler;
            if (rDownloadHandler == null)
            {
                rRequest.SetResult(rRequest);
                yield break;
            }

            rRequest.IsDone = true;
            rRequest.Text = rDownloadHandler.text;
            rRequest.Bytes = rDownloadHandler.data;

            rRequest.SetResult(rRequest);

            rWebRequest.Dispose();
            rDownloadHandler.Dispose();
            rWebRequest = null;
            rDownloadHandler = null;
        }
    }
}

