using Knight.GameSave.NinoGen;
using Nino.Core;
using System;
using System.IO;
using System.Reflection;

namespace Knight.Framework.GameSave
{
    public class GameSaveDataInfo
    {
        public string FileName;
        public string FilePath;
        public FieldInfo FieldInfo;
    }

    [NinoType]
    public partial class GameSaveData
    {
        [NinoIgnore]
        private bool mIsDirty;

        public void MarkDirty()
        {
            this.mIsDirty = true;
        }

        public void ClearDirty()
        {
            this.mIsDirty = false;
        }

        public bool IsDirty()
        {
            return this.mIsDirty;
        }

        public static void Load(string rSaveFileName, out GameSaveData rGameSaveData)
        {
            var rGameSaveFilePath = GameSavePath.GetSavePath(rSaveFileName);
            if (File.Exists(rGameSaveFilePath))
            {
                using (var fs = new GameSaveDecryptFileStream(rGameSaveFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (var br = new BinaryReader(fs))
                    {
                        var rDataBytes = br.ReadBytes((int)fs.Length);
                        Deserializer.Deserialize(rDataBytes.AsSpan<byte>(), out rGameSaveData);
                    }
                }
            }
            else
            {
                rGameSaveData = null;
            }
        }

        public static void Save(string rSaveFileName, ReadOnlySpan<byte> rGameSaveDataBytes)
        {
            var rGameSaveFilePath = GameSavePath.GetSavePath(rSaveFileName);
            using (var fs = new GameSaveDecryptFileStream(rGameSaveFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(rGameSaveDataBytes);
                }
            }
        }
    }
}
