//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using System.IO;
using Core.WindJson;
using System.Threading.Tasks;

namespace UnityEngine.AssetBundles
{
    public class ABServerURL
    {
        public string UpdateURL;
    }

    /// <summary>
    /// 资源管理类，主要用作不同资源平台的路径管理
    /// </summary>
    public class ABPlatform : TSingleton<ABPlatform>
    {
        /// <summary>
        /// 暂时选用如下平台，如需新平台再添加
        /// </summary>
        public enum Platform
        {
            OSXEditor = 0,
            OSXPlayer,
            WindowsPlayer,
            WindowsEditor,
            IphonePlayer,
            Android,
            Unkown,
        }
    
        /// <summary>
        /// 当前的平台
        /// </summary>
        public Platform         CurRuntimePlatform;
    
        /// <summary>
        /// 平台名
        /// </summary>
        public static string[]  PlatformNames = 
        {
            "OSX",
            "OSX",
            "Windows",
            "Windows",
            "IOS",
            "Android"
        };
    
        /// <summary>
        /// 运行平台是否为Editor
        /// </summary>
        public static bool[]    PlatformIsEditor = 
        {
            true,
            false,
            false,
            true,
            false,
            false
        };
    
        public static string[]  PlatformPrefixs = 
        {
            "file:///",         //OSXEditor
            "file:///",         //OSXPlayer
            "file:///",         //WindowsPlayer
            "file:///",         //WindowsEditor
            "file://",          //IphonePlayer
            "",                 //Android
        };

        public static string    IsDevelopeModeKey           = "ABPlatformEditor_IsDevelopeMode";
        public static string    IsSimulateModeKey_Scene     = "ABPlatformEditor_IsSimulateMode_Scene";
        public static string    IsSimulateModeKey_Avatar    = "ABPlatformEditor_IsSimulateMode_Avatar";
        public static string    IsSimulateModeKey_Config    = "ABPlatformEditor_IsSimulateMode_Config";
        public static string    IsSimulateModeKey_GUI       = "ABPlatformEditor_IsSimulateMode_GUI";
        public static string    IsSimulateModeKey_Script    = "ABPlatformEditor_IsSimulateMode_Script";

        /*******************************************************************************************/
        /// <summary>
        /// 更新服务器的配置
        /// </summary>
        public ABServerURL      ServerURL;

        private ABPlatform()
        {
        }
    
        /// <summary>
        /// 管理器的初始化
        /// </summary>
        public async Task Initialize()
        {
            Debug.LogFormat("IsDevelopeMode: {0}",        this.IsDevelopeMode());
            Debug.LogFormat("IsSumilateMode Scene: {0}",  this.IsSumilateMode_Scene());
            Debug.LogFormat("IsSumilateMode Avatar: {0}", this.IsSumilateMode_Avatar());
            Debug.LogFormat("IsSumilateMode Config: {0}", this.IsSumilateMode_Config());
            Debug.LogFormat("IsSumilateMode GUI: {0}",    this.IsSumilateMode_GUI());
            Debug.LogFormat("IsSumilateMode Script: {0}", this.IsSumilateMode_Script());

            this.CurRuntimePlatform = RuntimePlatform_To_Plaform(Application.platform);

            // 加载更新服务器的地址
            await LoadServerURL_Async();
        }

        /// <summary>
        /// 得到StreamingAssets下的资源目录路径
        /// </summary>
        public string GetStreamingFile(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;

            bool isEditor = PlatformIsEditor[rPlatformIndex];
            string rRootDir = isEditor ? Application.dataPath : Application.streamingAssetsPath;
            string rPlatformName = PlatformNames[rPlatformIndex];
#if UNITY_ANDROID && UNITY_EDITOR
            rPlatformName = "Android";
#endif
            return rRootDir + "/Assetbundles/" + rPlatformName + "_Assetbundles/";
        }

        /// <summary>
        /// 得到StreamingAssets下的资源目录的URL
        /// </summary>
        public string GetStreamingUrl(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
            return PlatformPrefixs[rPlatformIndex] + GetStreamingFile(rPlatform);
        }

        /// <summary>
        /// 得到当前平台的资源的URL
        /// </summary>
        public string GetStreamingUrl_CurPlatform(string rFileName)
        {
            string rPath = GetStreamingUrl(this.CurRuntimePlatform) + rFileName;
            return rPath;
        }

        /// <summary>
        /// 得到当前平台的资源的文件路径
        /// </summary>
        public string GetStreamingFile_CurPlatform(string rFileName)
        {
            string rPath = GetStreamingFile(this.CurRuntimePlatform) + rFileName;
            return rPath;
        }

        /// <summary>
        /// 得到AssetbundleManifest的Url路径
        /// </summary>
        public string GetAssetbundleManifestUrl()
        {
            string rRootPath = GetStreamingUrl(this.CurRuntimePlatform);
            DirectoryInfo rDirInfo = new DirectoryInfo(rRootPath);
            return rRootPath + rDirInfo.Name;
        }

        /// <summary>
        /// 得到Persistent空间下的文件路径
        /// </summary>
        public string GetPersistentFile(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
            return UtilTool.PathCombine(Application.persistentDataPath, PlatformNames[rPlatformIndex] + "_Assetbundles") + "/";
        }

        /// <summary>
        /// 得到Persistent空间下的Url路径
        /// </summary>
        public string GetPersistentUrl(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
            return PlatformPrefixs[rPlatformIndex] + UtilTool.PathCombine(Application.persistentDataPath, PlatformNames[rPlatformIndex] + "_Assetbundles") + "/";
        }

        /// <summary>
        /// 得到当前平台Persistent空间下的Url路径
        /// </summary>
        public string GetPersistentUrl_CurPlatform(string rFileName)
        {
            string rPath = GetPersistentUrl(this.CurRuntimePlatform) + rFileName;
            return rPath;
        }
        
        /// <summary>
        /// 得到当前平台Persistent空间下的文件路径
        /// </summary>
        public string GetPersistentFile_CurPlatform(string rFileName)
        {
            string rPath = GetPersistentFile(this.CurRuntimePlatform) + rFileName;
            return rPath;
        }

        /// <summary>
        /// 得到服务器的Url路径
        /// </summary>
        public string GetServerUrl(Platform rPlatform)
        {
            int rPlatformIndex = (int)rPlatform;
            return this.ServerURL.UpdateURL + PlatformNames[rPlatformIndex] + "_Assetbundles/";
        }

        /// <summary>
        /// 得到当前平台服务器的Url路径
        /// </summary>
        public string GetServerUrl_CurPlatform(string rFileName)
        {
            string rPath = GetServerUrl(this.CurRuntimePlatform) + rFileName;
            return rPath;
        }

        /// <summary>
        /// 是不是开发者模式
        /// </summary>
        public bool IsDevelopeMode()
        {
            bool bIsDevelopeMode = false;
#if UNITY_EDITOR
            bIsDevelopeMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsDevelopeModeKey);
#endif
            return bIsDevelopeMode;
        }

        /// <summary>
        /// 场景是不是模拟资源模式
        /// </summary>
        public bool IsSumilateMode_Scene()
        {
            bool bIsSimulateMode = false;
#if UNITY_EDITOR
            bIsSimulateMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Scene);
#endif
            return bIsSimulateMode && this.IsDevelopeMode();
        }
        
        /// <summary>
        /// 角色是不是模拟资源模式
        /// </summary>
        public bool IsSumilateMode_Avatar()
        {
            bool bIsSimulateMode = false;
#if UNITY_EDITOR
            bIsSimulateMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Avatar);
#endif
            return bIsSimulateMode && this.IsDevelopeMode();
        }

        /// <summary>
        /// 配置是不是模拟资源模式
        /// </summary>
        public bool IsSumilateMode_Config()
        {
            bool bIsSimulateMode = false;
#if UNITY_EDITOR
            bIsSimulateMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Config);
#endif
            return bIsSimulateMode && this.IsDevelopeMode();
        }

        /// <summary>
        /// GUI是不是模拟资源模式
        /// </summary>
        public bool IsSumilateMode_GUI()
        {
            bool bIsSimulateMode = false;
#if UNITY_EDITOR
            bIsSimulateMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_GUI);
#endif
            return bIsSimulateMode && this.IsDevelopeMode();
        }

        /// <summary>
        /// Script是不是模拟资源模式
        /// </summary>
        public bool IsSumilateMode_Script()
        {
            bool bIsSimulateMode = false;
#if UNITY_EDITOR
            bIsSimulateMode = UnityEditor.EditorPrefs.GetBool(ABPlatform.IsSimulateModeKey_Script);
#endif
            return bIsSimulateMode && this.IsDevelopeMode();
        }

        /// <summary>
        /// 加载更新服务器的地址
        /// </summary>
        private async Task LoadServerURL_Async()
        {
            var rResourceRequest = Resources.LoadAsync<TextAsset>("server_url_default");
            await rResourceRequest;

            string rServerContent = (rResourceRequest.asset as TextAsset).text;

            JsonNode rJsonNode = JsonParser.Parse(rServerContent);
            this.ServerURL = rJsonNode.ToObject<ABServerURL>();
        }

        /// <summary>
        /// 平台的对应关系转换
        /// </summary>
        private Platform RuntimePlatform_To_Plaform(RuntimePlatform rRuntimePlatform)
        {
            switch (rRuntimePlatform)
            {
                case RuntimePlatform.Android:       return Platform.Android;
                case RuntimePlatform.IPhonePlayer:  return Platform.IphonePlayer;
                case RuntimePlatform.OSXEditor:     return Platform.OSXEditor;
                case RuntimePlatform.OSXPlayer:     return Platform.OSXPlayer;
                case RuntimePlatform.WindowsEditor: return Platform.WindowsEditor;
                case RuntimePlatform.WindowsPlayer: return Platform.WindowsPlayer;
                default: return Platform.Unkown;
            }
        }
    }
}