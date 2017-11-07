//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Core;
using System.Threading.Tasks;

namespace Framework
{
    /// <summary>
    /// 全局数据类
    /// </summary>
    public class GameFlowLevelManager : TSingleton<GameFlowLevelManager>
    {
        public class LevelRequest
        {
            public Scene    Level;
            public string   LevelName;

            public LevelRequest(string rLevelName)
            {
                this.LevelName = rLevelName;
            }
        }

        GameFlowLevelManager() {}

        public async Task<LevelRequest> LoadLevel(string rLevelName)
        {
            var rLevelRequest = new LevelRequest(rLevelName);
            await SceneManager.LoadSceneAsync(rLevelRequest.LevelName);
            rLevelRequest.Level = SceneManager.GetSceneByName(rLevelRequest.LevelName);
            return rLevelRequest;
        }
        
        public Camera GetMainCamera()
        {
            var rMainCameraGo = GameObject.FindGameObjectWithTag("MainCamera");
            return rMainCameraGo.GetComponent<Camera>();
        }
    }
}