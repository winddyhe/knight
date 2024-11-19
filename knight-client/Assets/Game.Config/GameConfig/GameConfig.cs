using Cysharp.Threading.Tasks;
using Knight.Core;
using Knight.Framework.Serializer;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Game
{
    public partial class GameConfig
    {
        [SBIgnore]
        public static GameConfig Instance => Knight.Core.Singleton<GameConfig>.Instance;

        [SBIgnore]
        private string mConfigABPath = "game/config/gameconfig/binary.ab";

        public async UniTask Initialize()
        {
#if UNITY_EDITOR
            if (AssetLoader.Instance.IsDevelopMode)
            {
                this.LoadFromJsonInEditor("Assets/GameAssets/Config/GameConfig/Json");
            }
            else
            {
                await this.LoadFromBinary(this.mConfigABPath);
            }
#else
            await this.LoadFromBinary(this.mConfigABPath);
#endif
        }

        public string GetLanguage(MultiLanguageType rSystemLanguage, string rLanuageID)
        {
            if (!this.Language.Table.TryGetValue(rLanuageID, out var rLanguageConfig))
            {
                return rLanuageID.ToString();
            }
            switch (rSystemLanguage)
            {
                case MultiLanguageType.ChineseSimplified:
                    return rLanguageConfig.ChineseSimplified;
                case MultiLanguageType.ChineseTraditional:
                    return rLanguageConfig.ChineseTraditional;
                case MultiLanguageType.English:
                    return rLanguageConfig.English;
                case MultiLanguageType.Japanese:
                    return rLanguageConfig.Japanese;
                case MultiLanguageType.Indonesian:
                    return rLanguageConfig.Indonesian;
                case MultiLanguageType.Korean:
                    return rLanguageConfig.Korean;
                case MultiLanguageType.Malay:
                    return rLanguageConfig.Malay;
                case MultiLanguageType.Thai:
                    return rLanguageConfig.Thai;
                case MultiLanguageType.Arabic:
                    return rLanguageConfig.Arabic;
                default:
                    return rLanguageConfig.ChineseSimplified;
            }
        }

        public async UniTask LoadFromBinary(string rABPath)
        {
            var rConfigLoaderRequest = AssetLoader.Instance.LoadAllAssetAsync<TextAsset>(rABPath, AssetLoader.Instance.IsSimulate(ABSimuluateType.Config));
            await rConfigLoaderRequest.Task();

            if (rConfigLoaderRequest.AllAssets == null || rConfigLoaderRequest.AllAssets.Length == 0)
            {
                LogManager.LogErrorFormat("Load config binary failed, ABPath: {0}", rABPath);
                return;
            }

            var rGameConfigType = this.GetType();
            for (int i = 0; i < rConfigLoaderRequest.AllAssets.Length; i++)
            {
                var rConfigAsset = rConfigLoaderRequest.AllAssets[i];
                var rFieldInfo = rGameConfigType.GetField(rConfigAsset.name);
                if (rFieldInfo == null)
                {
                    LogManager.LogErrorFormat("Load config binary failed, can not find field: {0}", rConfigAsset.name);
                    continue;
                }
                var rConfigInstance = ReflectTool.Construct(rFieldInfo.FieldType);
                rFieldInfo.SetValue(this, rConfigInstance);
                ReflectTool.MethodMember(rConfigInstance, "Read", ReflectTool.flags_method_inst, rConfigAsset.bytes);
            }
        }

#if UNITY_EDITOR
        public void LoadFromJsonInEditor(string rJsonDirPath)
        {
            var rGameConfigType = this.GetType();
            var rAllFieldInfos = rGameConfigType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);
            for (int i = 0; i < rAllFieldInfos.Length; i++)
            {
                var rFieldInfo = rAllFieldInfos[i];
                var rJsonPathAttr = rFieldInfo.GetCustomAttribute<JsonPathAttribute>();
                if (rJsonPathAttr == null) continue;

                var rFiledObject = ReflectTool.Construct(rFieldInfo.FieldType);
                var rTableFieldInfo = rFieldInfo.FieldType.GetField("Table");
                var rJsonPath = PathTool.Combine(rJsonDirPath, rJsonPathAttr.Path + ".json");
                var rJsonText = File.ReadAllText(rJsonPath);
                var rFieldTableObject = JsonConvert.DeserializeObject(rJsonText, rTableFieldInfo.FieldType);

                rTableFieldInfo.SetValue(rFiledObject, rFieldTableObject);
                rFieldInfo.SetValue(this, rFiledObject);
            }
        }

        public void SaveToBinaryFiles(string rBinaryDirPath)
        {
            var rGameConfigType = this.GetType();
            var rAllFieldInfos = rGameConfigType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            for (int i = 0; i < rAllFieldInfos.Length; i++)
            {
                var rFieldInfo = rAllFieldInfos[i];
                var rJsonPathAttr = rFieldInfo.GetCustomAttribute<JsonPathAttribute>();
                if (rJsonPathAttr == null) continue;

                var rFieldValueObject = rFieldInfo.GetValue(this);
                var rBytesPath = PathTool.Combine(rBinaryDirPath, rJsonPathAttr.Path + ".bytes");
                ReflectTool.MethodMember(rFieldValueObject, "Save", ReflectTool.flags_method_inst, rBytesPath);
            }
        }

        public void SaveToBinaryFileByJsonFile(string rJsonDirPath, string rBinaryDirPath, string rJsonFileName)
        {
            var rGameConfigType = this.GetType();
            var rAllFieldInfos = rGameConfigType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);

            var rFiledInfo = rAllFieldInfos.FirstOrDefault(rInfo => rInfo.GetCustomAttribute<JsonPathAttribute>()?.Path == rJsonFileName);
            if (rFiledInfo == null) return;
            var rTableFieldInfo = rFiledInfo.FieldType.GetField("Table");
            if (rTableFieldInfo == null) return;

            var rJsonPath = PathTool.Combine(rJsonDirPath, rJsonFileName + ".json");
            var rJsonText = File.ReadAllText(rJsonPath);
            var rFieldTableObject = JsonConvert.DeserializeObject(rJsonText, rTableFieldInfo.FieldType);

            var rFiledObject = ReflectTool.Construct(rFiledInfo.FieldType);
            rTableFieldInfo.SetValue(rFiledObject, rFieldTableObject);

            var rBytesPath = PathTool.Combine(rBinaryDirPath, rJsonFileName + ".bytes");
            ReflectTool.MethodMember(rFiledObject, "Save", ReflectTool.flags_method_inst, rBytesPath);
        }
#endif
    }
}
