using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Knight.Core;
using Knight.Framework.Serializer;
using UnityEngine;

namespace Knight.Framework.Assetbundle
{
    public class ABUpdater : TSingleton<ABUpdater>
    {
        private string mABVersionMD5_Server;
        private string mABVersionMD5_Persistent;
        private string mABVersionMD5_Streaming;

        private ABVersion mABVersion_Server;
        private ABVersion mABVersion_Persistent;
        private ABVersion mABVersion_Streaming;

        private string mABVersion_PersistentPath;
        private string mABVersionMD5_PersistentPath;

        private ABUpdater()
        {
        }

        public async UniTask Initialize()
        {
            var rCdnVersionCode = ABPlatform.Instance.CdnVersionCode;

            var rABVersionMD5_StreamingURL = ABPlatform.Instance.GetStreamingUrl_CurPlatform("ABVersionMD5.bytes");
            this.mABVersionMD5_Streaming = await WebRequestTool.ReadContent(rABVersionMD5_StreamingURL);

            this.mABVersionMD5_PersistentPath = ABPlatform.Instance.GetPersistentFile_CurPlatform(rCdnVersionCode, "ABVersionMD5.bytes");
            this.mABVersionMD5_Persistent = PathTool.ReadFileText(this.mABVersionMD5_PersistentPath);

            // Persistent版本文件加载
            this.mABVersion_PersistentPath = ABPlatform.Instance.GetPersistentFile_CurPlatform(rCdnVersionCode, "ABVersion.bytes");
            var rPersistentBytes = PathTool.ReadFileBytes(this.mABVersion_PersistentPath);
            if (rPersistentBytes != null)
            {
                try
                {
                    this.mABVersion_Persistent = new ABVersion();
                    TSerializerBinaryReadWriteHelper.Read(this.mABVersion_Persistent, rPersistentBytes);
                }
                catch (Exception)
                {
                }
            }

            // Streaming版本文件加载
            var rABVersion_StreamingURL = ABPlatform.Instance.GetStreamingUrl_CurPlatform("ABVersion.bytes");
            var rStreamingBytes = await WebRequestTool.ReadContentBytes(rABVersion_StreamingURL);
            if (rStreamingBytes != null)
            {
                this.mABVersion_Streaming = new ABVersion();
                TSerializerBinaryReadWriteHelper.Read(this.mABVersion_Streaming, rStreamingBytes);
            }

            // 生成加载版本文件
            this.GenerateABLoadEntryVersion();
        }

        public async UniTask InitializeUpdate(Func<int, long, Task<bool>> rCheckUpdateAction, Action<long, long, long, float> rUpdateProgressAction)
        {
            if (!AssetLoader.Instance.IsDevelopMode)
            {
                // 下载服务器版本信息文件
                var rABVersionMD5_ServerURLInfo = ABPlatform.Instance.GetServerUrl("ABVersionMD5.bytes");
                this.mABVersionMD5_Server = await WebRequestTool.DownloadContent(rABVersionMD5_ServerURLInfo);
                
                // 开始下载更新文件
                await this.UpdateCheck(rCheckUpdateAction, rUpdateProgressAction);
            }

            // 重新生成加载版本文件
            this.GenerateABLoadEntryVersion();
        }

        public void GenerateABLoadEntryVersion()
        {
            if (AssetLoader.Instance.IsDevelopMode) return;

            ABLoadVersion.Instance.Initialize();
            foreach (var rPair in this.mABVersion_Streaming.Entries)
            {
                var rStreamingABEntry = rPair.Value;
                if (this.GetABEntryInABVersion(this.mABVersion_Persistent, rStreamingABEntry.ABPath, out var rPersistentABEntry))
                {
                    if (rPersistentABEntry.Version > rStreamingABEntry.Version)
                    {
                        // 如果是不需要删除的AB包，才需要加到加载列表中
                        if (!rPersistentABEntry.IsDeleteAB)
                        {
                            ABLoadVersion.Instance.AddEntry(rPersistentABEntry, ABLoadSpace.Presistent);
                        }
                    }
                    else
                    {
                        ABLoadVersion.Instance.AddEntry(rStreamingABEntry, ABLoadSpace.Streaming);
                    }
                }
                else
                {
                    ABLoadVersion.Instance.AddEntry(rStreamingABEntry, ABLoadSpace.Streaming);
                }
            }
            // 找到所有Persistent下有且Streaming没有的AB包
            if (this.mABVersion_Persistent != null)
            {
                foreach (var rPair in this.mABVersion_Persistent.Entries)
                {
                    var rPersistentABEntry = rPair.Value;
                    if (!this.GetABEntryInABVersion(this.mABVersion_Streaming, rPersistentABEntry.ABPath, out var rStreamingABEntry))
                    {
                        if (!rPersistentABEntry.IsDeleteAB)
                        {
                            ABLoadVersion.Instance.AddEntry(rPersistentABEntry, ABLoadSpace.Presistent);
                        }
                    }
                }
            }
        }

        private async UniTask UpdateCheck(Func<int, long, Task<bool>> rUpdateCheckAction, Action<long, long, long, float> rUpdateProgressAction)
        {
            // 如果没有服务器的MD5文件，或者服务器的MD5文件和本地的MD5文件一致，就不需要更新
            if (string.IsNullOrEmpty(this.mABVersionMD5_Server) || this.mABVersionMD5_Server.Equals(this.mABVersionMD5_Persistent))
            {
                return;
            }

            // 服务器版本文件下载
            var rABVerson_ServerURLInfo = ABPlatform.Instance.GetServerUrl("ABVersion.bytes");
            var rServerBytes = await WebRequestTool.DownloadContentBytes(rABVerson_ServerURLInfo);
            if (rServerBytes == null)
            {
                Debug.LogError($"ABVersion.bytes is null, {rABVerson_ServerURLInfo.MainURL}, {rABVerson_ServerURLInfo.SubURL}.");
                return;
            }
            this.mABVersion_Server = new ABVersion();
            TSerializerBinaryReadWriteHelper.Read(this.mABVersion_Server, rServerBytes);

            // 计算有多少文件需要下载
            var rNeedUpdateEntry = new List<ABEntry>();
            foreach (var rABServerEntryPair in this.mABVersion_Server.Entries)
            {
                var rServerABEntry = rABServerEntryPair.Value;
                if (this.GetABEntryInABVersion(this.mABVersion_Persistent, rServerABEntry.ABPath, out var rPersistentABEntry))
                {
                    if (!rServerABEntry.MD5.Equals(rPersistentABEntry.MD5) || rServerABEntry.Version > rPersistentABEntry.Version)
                    {
                        rNeedUpdateEntry.Add(rServerABEntry);
                    }
                }
                else
                {
                    if (this.GetABEntryInABVersion(this.mABVersion_Streaming, rServerABEntry.ABPath, out var rStreamingABEntry))
                    {
                        if (!rServerABEntry.MD5.Equals(rStreamingABEntry.MD5) || rServerABEntry.Version > rStreamingABEntry.Version)
                        {
                            rNeedUpdateEntry.Add(rServerABEntry);
                        }
                    }
                }
            }

            // 计算总共下载包的大小
            var nTotalSize = 0L;
            foreach (var rABEntry in rNeedUpdateEntry)
            {
                if (!rABEntry.IsDeleteAB)
                    nTotalSize += rABEntry.Size;
            }

            var bUpdateResult = await rUpdateCheckAction(rNeedUpdateEntry.Count, nTotalSize);
            if (!bUpdateResult)
            {
                // 如果不更新，直接返回
                return;
            }

            // 下载资源包，并且下载完更新本地的版本文件
            long nCurUpdateSize = 0;
            for (int i = 0; i < rNeedUpdateEntry.Count; i++)
            {
                var rABEntry = rNeedUpdateEntry[i];
                var rABEntrySavePath = ABPlatform.Instance.GetPersistentFile_CurPlatform(ABPlatform.Instance.CdnVersionCode, rABEntry.ABPath);
                if (rABEntry.IsDeleteAB)
                {
                    PathTool.DeleteFile(rABEntrySavePath);
                    this.UpdateABEntryInPersistentABVersion(rABEntry);
                    continue;
                }
                var rABEntryURLInfo = ABPlatform.Instance.GetServerUrl(rABEntry.ABPath);
                var bResult = await WebRequestTool.DowloadFile(rABEntryURLInfo, rABEntrySavePath, (float fUpdateProgress) => 
                {
                    rUpdateProgressAction?.Invoke(nTotalSize, nCurUpdateSize, rABEntry.Size, fUpdateProgress);
                });
                if (!bResult)
                {
                    Debug.LogError($"ABEntry can not download: {rABEntry.ABPath}, {rABEntryURLInfo.MainURL}, {rABEntryURLInfo.SubURL}.");
                    continue;
                }
                this.UpdateABEntryInPersistentABVersion(rABEntry);
                rUpdateProgressAction?.Invoke(nTotalSize, nCurUpdateSize, rABEntry.Size, 1.0f);
                nCurUpdateSize += rABEntry.Size;
            }
            LogManager.LogRelease("Res update complete...");
        }

        private void UpdateABEntryInPersistentABVersion(ABEntry rNewABEntry)
        {
            if (this.mABVersion_Persistent == null)
            {
                this.mABVersion_Persistent = new ABVersion();
                this.mABVersion_Persistent.Entries = new Dictionary<string, ABEntry>
                {
                    { rNewABEntry.ABPath, rNewABEntry }
                };
            }
            else
            {
                if (!this.mABVersion_Persistent.Entries.TryGetValue(rNewABEntry.ABPath, out var rPersistentABEntry))
                {
                    this.mABVersion_Persistent.Entries.Add(rNewABEntry.ABPath, rNewABEntry);
                }
                else
                {
                    rPersistentABEntry.Clone(rNewABEntry);
                }
            }
            TSerializerBinaryReadWriteHelper.Save(this.mABVersion_Persistent, this.mABVersion_PersistentPath);
        }

        private bool GetABEntryInABVersion(ABVersion rABVersion, string rABPath, out ABEntry rABEntry)
        {
            rABEntry = null;
            if (rABVersion == null) return false;
            if (rABVersion.Entries.TryGetValue(rABPath, out rABEntry))
            {
                return true;
            }
            return false;
        }
    }
}
