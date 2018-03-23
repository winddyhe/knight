//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using UnityEditor;

namespace Core.MultiScene.Editor
{
    /// <summary>
    /// 地形创建编辑器
    /// </summary>
    public class MultiSceneEditorWindow : EditorWindow
    {
        private MultiSceneEditor   __multiSceneEditor;
    
        [SerializeField]
        public Terrain              mTerrain;
    
        [MenuItem("Tools/Other/Multi Terrain Generator", priority = 1050)]
        private static void Init()
        {
            var rMultiSceneEditorWindow = EditorWindow.GetWindow<MultiSceneEditorWindow>();
            rMultiSceneEditorWindow.Show();
        }
    
        void Initialize()
        {
            __multiSceneEditor = new MultiSceneEditor(); 
        }
    
        void OnEnable() 
        {
            Initialize();
        }
    
        void OnGUI()
        {
            if (__multiSceneEditor == null) return;
    
            using (var space = new EditorGUILayout.VerticalScope())
            {
                EditorGUIUtility.labelWidth = 80;
                mTerrain = EditorGUILayout.ObjectField("地形：", mTerrain, typeof(Terrain), true) as Terrain;
                if (mTerrain == null) return;
    
                __multiSceneEditor.mTerrainData = mTerrain.terrainData;
                if (__multiSceneEditor.mTerrainData != null)
                {
                    using (var space1 = new EditorGUILayout.HorizontalScope())
                    {
                        GUI.enabled = false;
                        EditorGUILayout.IntField("地形的大小：", __multiSceneEditor.mTerrainData.heightmapWidth);
                        EditorGUIUtility.labelWidth = 15;
                        EditorGUILayout.IntField("*", __multiSceneEditor.mTerrainData.heightmapHeight);
                        GUI.enabled = true;
                    }
    
                    EditorGUIUtility.labelWidth = 80;
                    __multiSceneEditor.mTileSize = EditorGUILayout.IntField("分块大小：", __multiSceneEditor.mTileSize);
    
                    using (var space1 = new EditorGUILayout.HorizontalScope())
                    {
                        GUI.enabled = false;
                        __multiSceneEditor.mTileNumWidth = EditorGUILayout.IntField("地形块数：", __multiSceneEditor.mTerrainData.heightmapWidth / (__multiSceneEditor.mTileSize -1));
                        EditorGUIUtility.labelWidth = 15;
                        __multiSceneEditor.mTileNumHeight = EditorGUILayout.IntField("*", __multiSceneEditor.mTerrainData.heightmapHeight / (__multiSceneEditor.mTileSize -1));
                        GUI.enabled = true;
                    }
    
                    if (GUILayout.Button("分块", GUILayout.Width(50)))
                    {
                        __multiSceneEditor.TerrainTileCreate(new Vector2(mTerrain.transform.position.x, mTerrain.transform.position.z));
                    }
                }
            }
        }
    }
}