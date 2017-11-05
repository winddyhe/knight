//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.AssetBundles;
using System.Threading.Tasks;

namespace Framework
{
    public class SceneAssetLoader : TSingleton<SceneAssetLoader>
    {
        public class SceneLoaderRequest
        {
            public string        ABPath;
            public string        AssetName;
            public LoadSceneMode LoadMode;

            public SceneLoaderRequest(string rSceneABPath, string rSceneAssetName, LoadSceneMode rLoadSceneMode)
            {
                this.ABPath     = rSceneABPath;
                this.AssetName  = rSceneAssetName;
                this.LoadMode   = rLoadSceneMode;
            }
        }

        private SceneAssetLoader() { }

        public async Task<SceneLoaderRequest> Load_Async(string rSceneABPath, string rSceneName, LoadSceneMode rLoadSceneMode)
        {
            var rLoadRequest = new SceneLoaderRequest(rSceneABPath, rSceneName, rLoadSceneMode);

            GameObject rSceneConfigGo = null;
            if (ABPlatform.Instance.IsSumilateMode_Scene())
            {
                Debug.Log("---Simulate Load ab: " + rLoadRequest.ABPath);
#if UNITY_EDITOR
                if (rLoadRequest.LoadMode == LoadSceneMode.Additive)
                    await UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(rLoadRequest.AssetName);
                else
                    await UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(rLoadRequest.AssetName);
                
                var rScene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(rLoadRequest.AssetName));
                SceneManager.SetActiveScene(rScene);

                rSceneConfigGo = GameObject.Find(rScene.name + "_Config");
#endif
            }
            else
            {
                var rSceneRequest = await ABLoader.Instance.LoadScene(rLoadRequest.ABPath, rLoadRequest.AssetName);
                var rSceneLoadRequest = await SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(rSceneRequest.AssetName), rLoadRequest.LoadMode);
                rSceneRequest.Scene = SceneManager.GetSceneByName(Path.GetFileNameWithoutExtension(rSceneRequest.AssetName));
                SceneManager.SetActiveScene(rSceneRequest.Scene);

                rSceneConfigGo = GameObject.Find(rSceneRequest.Scene.name + "_Config");
            }

            if (rSceneConfigGo != null)
            {
                SceneConfig rSceneConfig = rSceneConfigGo.GetComponent<SceneConfig>();
                if (rSceneConfig != null)
                {
                    Camera rMainCamera = Camera.main;
                    rMainCamera.transform.position = rSceneConfig.CameraPos;
                    rMainCamera.transform.eulerAngles = rSceneConfig.CameraRotate;
                    rMainCamera.backgroundColor = rSceneConfig.CameraBGColor;
                    rMainCamera.fieldOfView = rSceneConfig.CameraFOV;
                    rMainCamera.farClipPlane = rSceneConfig.CameraFar;
                    rMainCamera.nearClipPlane = rSceneConfig.CameraNear;
                }
            }
            return rLoadRequest;
        }

        public void Unload(string rABPath)
        {
            ABLoader.Instance.UnloadAsset(rABPath);
        }
    }
}
