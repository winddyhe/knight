//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace Core
{
    public class WebRequestAssist
    {
        public class LoaderRequest : CoroutineRequest<LoaderRequest>
        {
            public string               Text;
            public byte[]               Bytes;
            public bool                 IsDone;

            public string               Url;
            public System.Action<float> DownloadProgress;

            public LoaderRequest(string rURL, System.Action<float> rDownloadProgress)
            {
                this.IsDone = false;
                this.Url = rURL;
                this.DownloadProgress = rDownloadProgress;
            }
        }

        public static LoaderRequest DownloadFile(string rURL, System.Action<float> rDownloadProgress = null)
        {
            LoaderRequest rRequest = new LoaderRequest(rURL, rDownloadProgress);
            rRequest.Start(DownloadFile_Async(rRequest));
            return rRequest;
        }

        private static IEnumerator DownloadFile_Async(LoaderRequest rRequest)
        {
            UnityWebRequest rWebRequest = UnityWebRequest.Get(rRequest.Url);
            CoroutineManager.Instance.Start(Record_DownloadProgress_Async(rRequest, rWebRequest));
            yield return rWebRequest.Send();

            if (rWebRequest.isNetworkError)
            {
                Debug.Log(rWebRequest.error);
                rWebRequest.Dispose();
                yield break;
            }

            var rDownloadHandler = rWebRequest.downloadHandler;
            if (rDownloadHandler == null)
            {
                yield break;
            }

            rRequest.IsDone = true;
            rRequest.Text = rDownloadHandler.text;
            rRequest.Bytes = rDownloadHandler.data;

            rWebRequest.Dispose();
            rDownloadHandler.Dispose();
            rWebRequest = null;
            rDownloadHandler = null;
        }

        private static IEnumerator Record_DownloadProgress_Async(LoaderRequest rRequest, UnityWebRequest rWebRequest)
        {
            while (!rRequest.IsDone)
            {
                try { UtilTool.SafeExecute(rRequest.DownloadProgress, rWebRequest.downloadProgress); }
                catch (System.Exception) {}

                yield return new WaitForEndOfFrame();
            }
            rWebRequest = null;
        }
    }
}

