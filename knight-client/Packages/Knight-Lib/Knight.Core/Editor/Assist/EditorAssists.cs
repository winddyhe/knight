//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using Object = UnityEngine.Object;
using System.Collections.Generic;

namespace Knight.Core.Editor
{
    public class EditorAssists
    {
        public static void RegisterUndo(string name, params Object[] objects)
        {
            if (objects != null && objects.Length > 0)
            {
                UnityEditor.Undo.RecordObjects(objects, name);
                foreach (var obj in objects)
                {
                    if (obj == null) continue;
                    EditorUtility.SetDirty(obj);
                }
            }
        }

        public static string ToMemorySize(long byteNum)
        {
            if (byteNum < 0)
                byteNum = 0;

            if (byteNum < 1024)
            {
                return byteNum + "B";
            }
            else if (byteNum < 1048576 && byteNum >= 1024) 
            {
                return (byteNum / 1024.0f).ToString("F2") + "KB";
            }
            else if (byteNum < 1073741824 && byteNum >= 1048576)
            {
                return (byteNum / 1048576.0f).ToString("F2") + "MB";
            }
            else
            {
                return (byteNum / 1073741824.0f).ToString("F2") + "GB";
            }
        }

        public static T ReceiveAsset<T>(string rAssetPath) where T : ScriptableObject
        {
            var rObj = AssetDatabase.LoadAssetAtPath<T>(rAssetPath) as T;
            if (rObj == null)
            {
                rObj = ScriptableObject.CreateInstance(typeof(T)) as T;
                AssetDatabase.CreateAsset(rObj, rAssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                rObj = AssetDatabase.LoadAssetAtPath<T>(rAssetPath) as T;
            }
            return rObj;
        }

        public static List<string> GetAssetPaths(string rSearch, string[] rPaths)
        {
            var rResultPaths = new List<string>();
            var rGUIDS = AssetDatabase.FindAssets(rSearch, rPaths);
            for (int i = 0; i < rGUIDS.Length; i++)
            {
                var rAssetPath = AssetDatabase.GUIDToAssetPath(rGUIDS[i]);
                rResultPaths.Add(rAssetPath);
            }
            return rResultPaths;
        }

        /// <summary>
        /// 加载Manifest
        /// </summary>
        public static IEnumerator LoadManifest(string rManifestPath, Action<AssetBundleManifest> rLoadCompleted)
        {
            var rABRequest = AssetBundle.LoadFromFileAsync(rManifestPath);
            yield return rABRequest;

            if (rABRequest == null || rABRequest.assetBundle == null)
            {
                Debug.Log("加载Manifest出错: " + rManifestPath);
                UtilTool.SafeExecute(rLoadCompleted, null);
                yield break;
            }
            var rABManifestRequest = rABRequest.assetBundle.LoadAssetAsync<AssetBundleManifest>(rManifestPath);
            yield return rABManifestRequest;
            if (rABManifestRequest.asset == null)
            {
                Debug.Log("加载Manifest出错：" + rManifestPath);
                yield break;
            }
            UtilTool.SafeExecute(rLoadCompleted, rABManifestRequest.asset as AssetBundleManifest);
            rABRequest.assetBundle.Unload(false);
        }

        public static AssetBundleManifest LoadManifest(string rManifestPath)
        {
            var rAssetBundle = AssetBundle.LoadFromFile(rManifestPath);
            if (rAssetBundle == null) return null;
            
            var rABManifest = rAssetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest") as AssetBundleManifest;
            rAssetBundle.Unload(false);
            return rABManifest;
        }
    }
}