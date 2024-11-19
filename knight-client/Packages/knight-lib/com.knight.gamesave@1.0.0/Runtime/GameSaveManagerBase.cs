using Knight.Core;
using Knight.GameSave.NinoGen;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;

namespace Knight.Framework.GameSave
{
    public class GameSaveManagerBase
    {
        protected Dictionary<GameSaveData, GameSaveDataInfo> mGameSaveDatas;
        protected Dictionary<string, GameSaveDataRequest> mGameSaveRequests;
        protected List<string> mGameSaveDataRequestRemoveList;

        protected JobHandle mGameSaveJobHandle;

        public void Initialize()
        {
            this.mGameSaveDatas = new Dictionary<GameSaveData, GameSaveDataInfo>();
            this.mGameSaveRequests = new Dictionary<string, GameSaveDataRequest>();
            this.mGameSaveDataRequestRemoveList = new List<string>();

            var rGameSaveManagerType = this.GetType();
            var rAllFieldInfos = rGameSaveManagerType.GetFields(ReflectTool.flags_public);
            for (int i = 0; i < rAllFieldInfos.Length; i++)
            {
                var rFieldInfo = rAllFieldInfos[i];
                if (rFieldInfo.IsDefined(typeof(GameSaveFileAttribute), false))
                {
                    var rGameSaveFileAttributes = rFieldInfo.GetCustomAttributes<GameSaveFileAttribute>(false);
                    if (rGameSaveFileAttributes == null || rGameSaveFileAttributes.Length == 0)
                    {
                        continue;
                    }
                    var rGameSaveFileAttribute = rGameSaveFileAttributes[0];
                    var rGameSaveFilePath = GameSavePath.GetSavePath(rGameSaveFileAttribute.FileName);

                    var rGameSaveData = ReflectTool.Construct(rFieldInfo.FieldType) as GameSaveData;
                    var rGameSaveDataInfo = new GameSaveDataInfo()
                    {
                        FileName = rGameSaveFileAttribute.FileName,
                        FilePath = rGameSaveFilePath,
                        FieldInfo = rFieldInfo,
                    };
                    this.mGameSaveDatas.Add(rGameSaveData, rGameSaveDataInfo);
                }
            }

            foreach (var rGameSaveDataPair in this.mGameSaveDatas)
            {
                var rGameSaveData = rGameSaveDataPair.Key;
                var rGameSaveFileInfo = rGameSaveDataPair.Value;

                GameSaveData.Load(rGameSaveFileInfo.FilePath, out rGameSaveData);
                rGameSaveFileInfo.FieldInfo.SetValue(this, rGameSaveData);
            }
        }

        public void Update()
        {
            // 检测哪些数据需要保存?
            foreach (var rGameSaveDataPair in this.mGameSaveDatas)
            {
                var rGameSaveData = rGameSaveDataPair.Key;
                var rGameSaveFileInfo = rGameSaveDataPair.Value;
                if (rGameSaveData.IsDirty())
                {
                    if (this.mGameSaveRequests.TryGetValue(rGameSaveFileInfo.FilePath, out var rGameSaveRequest) &&
                        !rGameSaveRequest.IsSaving && !rGameSaveRequest.IsSaveCompleted)
                    {
                        rGameSaveRequest.SaveDatas = new NativeArray<byte>(Serializer.Serialize(rGameSaveData), Allocator.Persistent);
                        rGameSaveRequest.SaveFileName = new FixedString32Bytes(rGameSaveFileInfo.FileName);
                        rGameSaveRequest.IsSaving = false;
                        rGameSaveRequest.IsSaveCompleted = false;
                    }
                    else
                    {
                        rGameSaveRequest = new GameSaveDataRequest()
                        {
                            SaveDatas = new NativeArray<byte>(Serializer.Serialize(rGameSaveData), Allocator.Persistent),
                            SaveFileName = new FixedString32Bytes(rGameSaveFileInfo.FileName),
                            IsSaving = false,
                            IsSaveCompleted = false,
                        };
                        this.mGameSaveRequests.Add(rGameSaveFileInfo.FilePath, rGameSaveRequest);
                    }
                    rGameSaveData.ClearDirty();
                }
            }

            // 使用JobSystem异步保存数据
            if (this.mGameSaveJobHandle.IsCompleted)
            {
                foreach (var rGameSaveRequestPair in this.mGameSaveRequests)
                {
                    var rGameSaveRequest = rGameSaveRequestPair.Value;
                    if (!rGameSaveRequest.IsSaving && !rGameSaveRequest.IsSaveCompleted)
                    {
                        rGameSaveRequest.IsSaving = true;
                        
                        this.mGameSaveJobHandle.Complete();
                        rGameSaveRequest.IsSaveCompleted = true;

                        var rGameSaveJob = new GameSaveJob()
                        {
                            SaveFileName = rGameSaveRequest.SaveFileName,
                            SaveDatas = rGameSaveRequest.SaveDatas,
                        };
                        this.mGameSaveJobHandle = rGameSaveJob.Schedule(this.mGameSaveJobHandle);
                    }
                }
            }

            // 删除保存请求
            this.mGameSaveDataRequestRemoveList.Clear();
            foreach (var rGameSaveRequestPair in this.mGameSaveRequests)
            {
                var rGameSaveRequest = rGameSaveRequestPair.Value;
                if (rGameSaveRequest.IsSaveCompleted)
                {
                    this.mGameSaveDataRequestRemoveList.Add(rGameSaveRequestPair.Key);
                }
            }
            foreach (var rGameSaveDataRequestRemove in this.mGameSaveDataRequestRemoveList)
            {
                var rGameSaveRequest = this.mGameSaveRequests[rGameSaveDataRequestRemove];
                rGameSaveRequest.SaveDatas.Dispose();
                this.mGameSaveRequests.Remove(rGameSaveDataRequestRemove);
            }
        }

        struct GameSaveJob : IJob
        {
            public FixedString32Bytes SaveFileName;
            public NativeArray<byte> SaveDatas;

            public void Execute()
            {
                GameSaveData.Save(this.SaveFileName.ToString(), this.SaveDatas.AsReadOnlySpan());
            }
        }
    }
}
