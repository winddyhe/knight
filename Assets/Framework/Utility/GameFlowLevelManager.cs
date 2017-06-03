//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Core;

namespace Framework
{
    /// <summary>
    /// 全局数据类
    /// </summary>
    public class GameFlowLevelManager : TSingleton<GameFlowLevelManager>
    {
        public class LevelRequest : CoroutineRequest<LevelRequest>
        {
            public Scene    Level;
            public string   LevelName;

            public LevelRequest(string rLevelName)
            {
                this.LevelName = rLevelName;
            }
        }

        GameFlowLevelManager() {}

        public LevelRequest LoadLevel(string rLevelName)
        {
            var rLevelRequest = new LevelRequest(rLevelName);
            rLevelRequest.Start(LoadLevel_async(rLevelRequest));
            return rLevelRequest;
        }

        private IEnumerator LoadLevel_async(LevelRequest rRequest)
        {
            yield return SceneManager.LoadSceneAsync(rRequest.LevelName);
            rRequest.Level = SceneManager.GetSceneByName(rRequest.LevelName);
        }

        public Camera GetMainCamera()
        {
            var rMainCameraGo = GameObject.FindGameObjectWithTag("MainCamera");
            return rMainCameraGo.GetComponent<Camera>();
        }
    }
}