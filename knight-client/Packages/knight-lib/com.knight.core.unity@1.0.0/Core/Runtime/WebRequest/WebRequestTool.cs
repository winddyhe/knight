using Cysharp.Threading.Tasks;
using System;
using UnityEngine.Networking;

namespace Knight.Core
{
    public static class WebRequestTool
    {
        public static async UniTask<string> ReadContent(string rUrl)
        {
            try
            {
                var rRequest = UnityWebRequest.Get(rUrl);
                await rRequest.SendWebRequest();
                if (rRequest.result != UnityWebRequest.Result.Success)
                {
                    LogManager.LogError($"WebRequestTool.GetContent Error: {rRequest.error}, {rUrl}");
                    rRequest.Dispose();
                    return string.Empty;
                }
                else
                {
                    // 成功了就返回
                    var rContent = rRequest.downloadHandler.text;
                    rRequest.Dispose();
                    return rContent;
                }
            }
            catch (Exception e)
            {
                LogManager.LogError($"WebRequestTool.GetContent Error: {rUrl}");
                LogManager.LogException(e);
                return string.Empty;
            }
        }

        public static async UniTask<byte[]> ReadContentBytes(string rUrl)
        {
            try
            {
                var rRequest = UnityWebRequest.Get(rUrl);
                await rRequest.SendWebRequest();

                if (rRequest.result != UnityWebRequest.Result.Success)
                {
                    LogManager.LogError($"WebRequestTool.GetContent Error: {rRequest.error}, {rUrl}");
                    rRequest.Dispose();
                    return null;
                }
                else
                {
                    var rContent = rRequest.downloadHandler.data;
                    rRequest.Dispose();
                    return rContent;
                }
            }
            catch (Exception e)
            {
                LogManager.LogException(e);
                return null;
            }
        }

        /// <summary>
        /// 适用于下载小文件直接获取其内容 字符串
        /// </summary>
        public static async UniTask<string> DownloadContent(UrlInfo rUrl)
        {
            if (rUrl == null) return string.Empty;

            var rContent = await DownloadContent(rUrl.MainURL);
            if (string.IsNullOrEmpty(rContent))
            {
                rContent = await DownloadContent(rUrl.SubURL);
            }
            return rContent;
        }

        private static async UniTask<string> DownloadContent(string rUrl)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var rRequest = UnityWebRequest.Get(rUrl);
                    rRequest.timeout = 3;
                    await rRequest.SendWebRequest();
                    if (rRequest.result != UnityWebRequest.Result.Success)
                    {
                        LogManager.LogError($"WebRequestTool.GetContent Error: {rRequest.error}, {rUrl}");
                        rRequest.Dispose();
                    }
                    else
                    {
                        // 成功了就返回
                        var rContent = rRequest.downloadHandler.text;
                        rRequest.Dispose();
                        return rContent;
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogException(e);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 适用于下载小文件直接获取其内容 字节数组
        /// </summary>
        public static async UniTask<byte[]> DownloadContentBytes(UrlInfo rUrl)
        {
            if (rUrl == null) return null;

            var rContent = await DownloadContentBytes(rUrl.MainURL);
            if (rContent == null)
            {
                rContent = await DownloadContentBytes(rUrl.SubURL);
            }
            return rContent;
        }

        private static async UniTask<byte[]> DownloadContentBytes(string rUrl)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var rRequest = UnityWebRequest.Get(rUrl);
                    rRequest.timeout = 3;
                    await rRequest.SendWebRequest();

                    if (rRequest.result != UnityWebRequest.Result.Success)
                    {
                        LogManager.LogError($"WebRequestTool.GetContent Error: {rRequest.error}, {rUrl}");
                        rRequest.Dispose();
                    }
                    else
                    {
                        var rContent = rRequest.downloadHandler.data;
                        rRequest.Dispose();
                        return rContent;
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogException(e);
                }
            }
            return null;
        }

        public static async UniTask<bool> DowloadFile(UrlInfo rUrlInfo, string rSavePath, Action<float> rProgressAction)
        {
            if (rUrlInfo == null) return false;

            var rResult = await DownloadFile(rUrlInfo.MainURL, rSavePath, rProgressAction);
            if (!rResult)
            {
                rResult = await DownloadFile(rUrlInfo.SubURL, rSavePath, rProgressAction);
            }
            return rResult;
        }

        /// <summary>
        /// 适用于下载文件并保存本地，返回是否保存成功
        /// </summary>
        private static async UniTask<bool> DownloadFile(string rUrl, string rSavePath, Action<float> rProgressAction)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    var rRequest = UnityWebRequest.Get(rUrl);
                    rRequest.timeout = 3;
                    rRequest.downloadHandler = new DownloadHandlerFile(rSavePath);
                    var rRequestOpt = rRequest.SendWebRequest();
                    while (!rRequestOpt.isDone)
                    {
                        rProgressAction?.Invoke(rRequestOpt.progress);
                        await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
                    }
                    if (rRequest.result != UnityWebRequest.Result.Success)
                    {
                        LogManager.LogError($"WebRequestTool.DownloadFile Error: {rRequest.error}, {rUrl}");
                        rRequest.Dispose();
                    }
                    else
                    {
                        rRequest.Dispose();
                        return true;
                    }
                }
                catch (Exception e)
                {
                    LogManager.LogException(e);
                }
            }
            return false;
        }
    }
}
