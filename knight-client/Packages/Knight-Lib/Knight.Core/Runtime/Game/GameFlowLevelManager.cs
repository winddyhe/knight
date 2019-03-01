//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityFx.Async;

namespace Knight.Core
{
    /// <summary>
    /// 全局数据类
    /// </summary>
    public class GameFlowLevelManager : TSingleton<GameFlowLevelManager>
    {
        public class LevelRequest : AsyncRequest<LevelRequest>
        {
            public Scene    Level;
            public string   LevelName;

            public LevelRequest(string rLevelName)
            {
                this.LevelName = rLevelName;
            }
        }

        GameFlowLevelManager() {}

        public IAsyncOperation<LevelRequest> LoadLevel(string rLevelName)
        {
            var rRequest = new LevelRequest(rLevelName);
            return rRequest.Start(LoadLevel(rRequest));
        }

        public IEnumerator LoadLevel(LevelRequest rRequest)
        {
            yield return SceneManager.LoadSceneAsync(rRequest.LevelName);
            rRequest.Level = SceneManager.GetSceneByName(rRequest.LevelName);
            rRequest.SetResult(rRequest);
        }

        public Camera GetMainCamera()
        {
            var rMainCameraGo = GameObject.FindGameObjectWithTag("MainCamera");
            return rMainCameraGo.GetComponent<Camera>();
        }
    }
}