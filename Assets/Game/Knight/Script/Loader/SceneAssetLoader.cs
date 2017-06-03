//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework;
using Core;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.AssetBundles;

namespace Game.Knight
{
    public class SceneAssetLoader : TSingleton<SceneAssetLoader>
    {
        public class SceneLoaderRequest : CoroutineRequest<SceneLoaderRequest>
        {
            public string        sceneABPath;
            public string        sceneAssetName;
            public GameObject    sceneRootGo;
            public LoadSceneMode sceneLoadMode;

            public SceneLoaderRequest(string rSceneABPath, string rSceneAssetName, LoadSceneMode rLoadSceneMode)
            {
                this.sceneABPath = rSceneABPath;
                this.sceneAssetName = rSceneAssetName;
                this.sceneLoadMode = rLoadSceneMode;
            }
        }

        private SceneAssetLoader() { }

        public SceneLoaderRequest Load_Async(string rSceneABPath, string rSceneName, LoadSceneMode rLoadSceneMode)
        {
            var rLoadRequest = new SceneLoaderRequest(rSceneABPath, rSceneName, rLoadSceneMode);
            rLoadRequest.Start(Load_Async(rLoadRequest));
            return rLoadRequest;
        }

        private IEnumerator Load_Async(SceneLoaderRequest rLoadRequest)
        {
            var rSceneRequest = ABLoader.Instance.LoadScene(rLoadRequest.sceneABPath, rLoadRequest.sceneAssetName);
            yield return rSceneRequest;

            string rSceneName = Path.GetFileNameWithoutExtension(rSceneRequest.AssetName);
            var rSceneLoadRequest = SceneManager.LoadSceneAsync(rSceneName, rLoadRequest.sceneLoadMode);
            yield return rSceneLoadRequest;

            rSceneRequest.Scene = SceneManager.GetSceneByName(rSceneName);
            SceneManager.SetActiveScene(rSceneRequest.Scene);

            GameObject rSceneConfigGo = GameObject.Find(rSceneRequest.Scene.name + "_Config");
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
            ABLoader.Instance.UnloadAsset(rLoadRequest.sceneABPath);
        }
    }
}
