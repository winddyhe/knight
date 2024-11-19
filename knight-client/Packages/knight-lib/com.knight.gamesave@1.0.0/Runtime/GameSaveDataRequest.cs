using Unity.Collections;

namespace Knight.Framework.GameSave
{
    public class GameSaveDataRequest
    {
        public FixedString32Bytes SaveFileName;
        public NativeArray<byte> SaveDatas;

        public bool IsSaving;
        public bool IsSaveCompleted;
    }
}
