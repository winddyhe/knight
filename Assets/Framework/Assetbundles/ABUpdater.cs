//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections.Generic;
using System.Collections;
using Core;
using System.IO;
using Framework;
using System.Threading.Tasks;

namespace UnityEngine.AssetBundles
{

    public class ABUpdater : TSingleton<ABUpdater>
    {
        public string           mStreamingMD5;
        public string           mPersistentMD5;
        public string           mServerMD5;

        public ABVersion        mStreamingVersion;
        public ABVersion        mPersistentVersion;
        public ABVersion        mServerVersion;
        
        private ABUpdater()
        {
        }

        public async Task Initialize()
        {
            GameLoading.Instance.StartLoading(1.0f, "游戏初始化阶段，开始检查资源更新...");

            // 加载Streaming空间下的版本MD5码
            var rStreamingMD5Request = WWWAssist.LoadFile(ABPlatform.Instance.GetStreamingUrl_CurPlatform(ABVersion.ABVersion_File_MD5));
            mStreamingMD5 = (await rStreamingMD5Request).Text;
            Debug.Log("--- Streaming MD5: " + mStreamingMD5);

            if (!ABPlatform.Instance.IsDevelopeMode())
            {   
                // 加载Persitent空间下的版本MD5码
                var rPersistentMD5Request = await WWWAssist.LoadFile(ABPlatform.Instance.GetPersistentUrl_CurPlatform(ABVersion.ABVersion_File_MD5));
                mPersistentMD5 = rPersistentMD5Request.Text;
                Debug.Log("--- Persistent MD5: " + mPersistentMD5);

                // 加载服务器上的版本MD5码
                var rServerMD5Request = await WebRequestAssist.DownloadFile(ABPlatform.Instance.GetServerUrl_CurPlatform(ABVersion.ABVersion_File_MD5));
                if (rServerMD5Request != null)
                    mServerMD5 = rServerMD5Request.Text;
                Debug.Log("--- Server MD5: " + mServerMD5);
                
                // 加载Persisntent空间的版本信息文件
                var rPersistentVersionRequest = await ABVersion.Load(ABPlatform.Instance.GetPersistentUrl_CurPlatform(ABVersion.ABVersion_File_Bin));
                if (rPersistentVersionRequest != null)
                    mPersistentVersion = rPersistentVersionRequest.Version;

                if (!string.IsNullOrEmpty(mServerMD5) && !mServerMD5.Equals(mPersistentMD5))
                {
                    GameLoading.Instance.Hide();

                    // 开始下载
                    await this.UpdateResource_Sync();
                }
            }

            GameLoading.Instance.StartLoading(0.2f, "游戏初始化阶段，开始加载资源...");

            // 加载Streaming空间的版本信息文件
            var rStreamingVersionRequest = await ABVersion.Load(ABPlatform.Instance.GetStreamingUrl_CurPlatform(ABVersion.ABVersion_File_Bin));
            mStreamingVersion = rStreamingVersionRequest.Version;

            // 生成最终用于资源加载的版本信息
            this.GenerateCombineVersion();

            GameLoading.Instance.Hide();
        }

        /// <summary>
        /// 开始从服务器更新资源
        /// </summary>
        private async Task UpdateResource_Sync()
        {
            GameLoading.Instance.StartLoading("游戏初始化阶段，开始下载更新的资源...");

            // 加载服务器的版本信息文件
            var rServerVersionRequest = await ABVersion.Download(ABPlatform.Instance.GetServerUrl_CurPlatform(ABVersion.ABVersion_File_Bin));
            mServerVersion = rServerVersionRequest.Version;

            if (mServerVersion == null) return;

            // 比较两个空间的版本信息
            List<ABVersionEntry> rNeedUpdateEntries = new List<ABVersionEntry>();
            foreach (var rPair in mServerVersion.Entries)
            {
                var rServerAVEntry = rPair.Value;

                if (mPersistentVersion != null)
                {
                    var rPersistentAVEntry = mPersistentVersion.GetEntry(rServerAVEntry.Name);
                    if (rPersistentAVEntry == null || rServerAVEntry.Version > rPersistentAVEntry.Version)
                    {
                        rNeedUpdateEntries.Add(rServerAVEntry);
                    }
                }
                else
                {
                    rNeedUpdateEntries.Add(rServerAVEntry);
                }
            }

            if (mPersistentVersion == null)
            {
                mPersistentVersion = new ABVersion();
                mPersistentVersion.Entries = new Dict<string, ABVersionEntry>();
            }

            for (int i = 0; i < rNeedUpdateEntries.Count; i++)
            {
                GameLoading.Instance.SetTips(string.Format("游戏初始化阶段，开始下载更新的资源[{0}/{1}]...", i + 1, rNeedUpdateEntries.Count));

                // 下载文件
                var rServerABRequest = await WebRequestAssist.DownloadFile(ABPlatform.Instance.GetServerUrl_CurPlatform(rNeedUpdateEntries[i].Name), GameLoading.Instance.SetLoadingProgress);

                // 写入文件
                string rPersisentFilePath = ABPlatform.Instance.GetPersistentFile_CurPlatform(rNeedUpdateEntries[i].Name);
                UtilTool.WriteAllBytes(rPersisentFilePath, rServerABRequest.Bytes);
                Debug.Log("--- Save ab: " + rPersisentFilePath);

                // 保存修改后的Persistent空间的文件
                var rPersistentAVEntry = mPersistentVersion.GetEntry(rNeedUpdateEntries[i].Name);
                if (rPersistentAVEntry == null)
                {
                    rPersistentAVEntry = new ABVersionEntry();
                    mPersistentVersion.Entries.Add(rNeedUpdateEntries[i].Name, rPersistentAVEntry);
                }
                rPersistentAVEntry.Name = rNeedUpdateEntries[i].Name;
                rPersistentAVEntry.Size = rNeedUpdateEntries[i].Size;
                rPersistentAVEntry.Version = rNeedUpdateEntries[i].Version;
                rPersistentAVEntry.MD5 = rNeedUpdateEntries[i].MD5;
                rPersistentAVEntry.Dependencies = new List<string>(rNeedUpdateEntries[i].Dependencies).ToArray();

                string rPersistentVersionPath = ABPlatform.Instance.GetPersistentFile_CurPlatform(ABVersion.ABVersion_File_Bin);
                string rPersistentVersionDirPath = Path.GetDirectoryName(rPersistentVersionPath);
                if (!Directory.Exists(rPersistentVersionDirPath)) Directory.CreateDirectory(rPersistentVersionDirPath);

                mPersistentVersion.Save(rPersistentVersionPath);
                Debug.Log("--- Save version: " + rPersistentVersionPath);
            }

            // 保存服务器上的MD5文件
            string rPersistentMD5Path = ABPlatform.Instance.GetPersistentFile_CurPlatform(ABVersion.ABVersion_File_MD5);
            UtilTool.WriteAllText(rPersistentMD5Path, mServerMD5);
            Debug.Log("--- Save md5: " + rPersistentMD5Path);

            GameLoading.Instance.Hide();
        }

        /// <summary>
        /// 生成最终用于资源加载的版本信息
        /// </summary>
        private void GenerateCombineVersion()
        {
            ABLoaderVersion.Instance.Initialize();

            foreach (var rPair in mStreamingVersion.Entries)
            {
                var rStreamingAVEntry = rPair.Value;
                if (mPersistentVersion != null)
                {
                    var rPersitentAVEntry = mPersistentVersion.GetEntry(rStreamingAVEntry.Name);
                    if (rPersitentAVEntry != null && rPersitentAVEntry.Version > rStreamingAVEntry.Version)
                    {
                        ABLoaderVersion.Instance.AddEntry(LoaderSpace.Persistent, rPersitentAVEntry);
                    }
                    else
                    {
                        ABLoaderVersion.Instance.AddEntry(LoaderSpace.Streaming, rStreamingAVEntry);
                    }
                }
                else
                {
                    ABLoaderVersion.Instance.AddEntry(LoaderSpace.Streaming, rStreamingAVEntry);
                }
            }

            if (mPersistentVersion != null)
            {
                foreach (var rPair in mPersistentVersion.Entries)
                {
                    var rPersitentAVEntry = rPair.Value;
                    var rStreamingAVEntry = mStreamingVersion.GetEntry(rPersitentAVEntry.Name);
                    if (rStreamingAVEntry == null)
                    {
                        ABLoaderVersion.Instance.AddEntry(LoaderSpace.Persistent, rPersitentAVEntry);
                    }
                }
            }
        }
    }
}
