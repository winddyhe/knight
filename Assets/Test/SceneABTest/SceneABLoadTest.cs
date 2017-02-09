//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

namespace Test
{
    public class SceneABLoadTest : MonoBehaviour
    {
        void Start()
        {
            this.StartCoroutine(Load_Async());
        }

        IEnumerator Load_Async()
        {
            WWW www = new WWW("file:///F:/Winddy/Unity/Game/WindUnityTool/Assets/Test/SceneABTest/ABS/scene_1.ab");
            yield return www;

            string rScenePath = www.assetBundle.GetAllScenePaths()[0];
            Debug.Log(rScenePath);
            SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(rScenePath), LoadSceneMode.Single);


            WWW www1 = new WWW("file:///F:/Winddy/Unity/Game/WindUnityTool/Assets/Test/SceneABTest/ABS/scene_2.ab");
            yield return www1;

            string rScenePath1 = www1.assetBundle.GetAllScenePaths()[0];
            Debug.Log(rScenePath1);
            SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(rScenePath1), LoadSceneMode.Additive);
        }
    }
}

