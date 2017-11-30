//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Threading.Tasks;

namespace Core
{
    public class WebRequestAssist
    {
        public class LoaderRequest
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

        public static async Task<LoaderRequest> DownloadFile(string rURL, System.Action<float> rDownloadProgress = null)
        {
            LoaderRequest rRequest = new LoaderRequest(rURL, rDownloadProgress);

            UnityWebRequest rWebRequest = UnityWebRequest.Get(rRequest.Url);
            var rProgressCoroutine = CoroutineManager.Instance.StartHandler(Record_DownloadProgress_Async(rRequest, rWebRequest));
            await rWebRequest.SendWebRequest();

            if (rWebRequest.isNetworkError)
            {
                Debug.Log(rWebRequest.error);
                rWebRequest.Dispose();
                return null;
            }

            var rDownloadHandler = rWebRequest.downloadHandler;
            if (rDownloadHandler == null)
            {
                return null;
            }

            rRequest.IsDone = true;
            rRequest.Text = rDownloadHandler.text;
            rRequest.Bytes = rDownloadHandler.data;

            rWebRequest.Dispose();
            rDownloadHandler.Dispose();
            rWebRequest = null;
            rDownloadHandler = null;
            CoroutineManager.Instance.Stop(rProgressCoroutine);

            return rRequest;
        }

        private static IEnumerator Record_DownloadProgress_Async(LoaderRequest rRequest, UnityWebRequest rWebRequest)
        {
            while (!rRequest.IsDone)
            {
                try { UtilTool.SafeExecute(rRequest.DownloadProgress, rWebRequest.downloadProgress); }
                catch (System.Exception) { break; }

                yield return new WaitForEndOfFrame();
            }
            rWebRequest = null;
        }
    }
}

