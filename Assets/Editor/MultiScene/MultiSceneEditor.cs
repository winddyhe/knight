//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.IO;

namespace Core.MultiScene.Editor
{
    public class MultiSceneEditor
    {
        public static readonly string MultiSceneDir = "Assets/OriginalAssets/Scenes/Resources/";
    
        [SerializeField]
        public int          mTileSize       = 65;
    
        public int          mTileNumWidth   = 8;
        public int          mTileNumHeight  = 8;
    
        public TerrainData  mTerrainData;
    
        /// <summary>
        /// 创建地形分块
        /// </summary>
        public void TerrainTileCreate(Vector2 rOffset)
        {
            if (mTerrainData == null) return;
    
            var rSplatMaps = mTerrainData.splatPrototypes;
            Scene rScene = EditorSceneManager.GetActiveScene();
    
            LargeScene rLargeScene = ScriptableObject.CreateInstance<LargeScene>();
            rLargeScene.ID = rScene.name;
            rLargeScene.Size = mTerrainData.size;
            rLargeScene.TileRows = mTileNumHeight;
            rLargeScene.TileCols = mTileNumWidth;
            rLargeScene.Offset = rOffset;
    
            // 保存大场景数据
            string rLargeSceneConfigPath = string.Format(MultiSceneDir + "MultiScene/{0}.asset", rScene.name);
            AssetDatabase.CreateAsset(rLargeScene, rLargeSceneConfigPath);
            AssetDatabase.Refresh();
    
            int k = 0;
            List<Terrain> rTerrains = new List<Terrain>();
            for (int i = 0; i < mTileNumHeight; i++)
            {
                for (int j = 0; j < mTileNumWidth; j++)
                {
                    TerrainData rTerrainData = new TerrainData();
                    rTerrainData.heightmapResolution = mTileSize;
                    rTerrainData.baseMapResolution = mTerrainData.baseMapResolution;
                    rTerrainData.SetDetailResolution(mTerrainData.detailResolution, 8);
                    float[,] rHeights = mTerrainData.GetHeights((mTileSize - 1) * i, (mTileSize - 1) * j, mTileSize, mTileSize);
                    rTerrainData.SetHeights(0, 0, rHeights);
                    rTerrainData.size = new Vector3(mTerrainData.size.x / mTileNumWidth, mTerrainData.size.y, mTerrainData.size.z / mTileNumHeight);
                    rTerrainData.splatPrototypes = CreateSplatPrototypes(rSplatMaps, mTerrainData.baseMapResolution, i, j);
                    float[,,] rAlphaMaps = mTerrainData.GetAlphamaps((mTileSize - 1) * i, (mTileSize - 1) * j, mTileSize - 1, mTileSize - 1);
    
                    string rTerrainPath = string.Format(MultiSceneDir + "{0}/tile_{1}_{2}.asset", rScene.name, i, j);
                    string rTerrainConfigPath = string.Format(MultiSceneDir + "{0}/tile_{1}_{2}_config.asset", rScene.name, i, j);
    
                    if (!Directory.Exists(Path.GetDirectoryName(rTerrainPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(rTerrainPath));
    
                    SceneTile rTile = ScriptableObject.CreateInstance<SceneTile>();
                    rTile.ID = k;
                    rTile.Row = i;
                    rTile.Col = j;
                    rTile.ParentID = rLargeScene.ID;
                    rTile.Size = mTileSize;
                    rTile.Offset = new Vector2(rTerrainData.size.x * i, rTerrainData.size.z * j);
    
                    AssetDatabase.CreateAsset(rTerrainData, rTerrainPath);
                    AssetDatabase.CreateAsset(rTile, rTerrainConfigPath);
                    AssetDatabase.Refresh();
    
                    rTerrainData = AssetDatabase.LoadAssetAtPath(rTerrainPath, typeof(TerrainData)) as TerrainData;
                    var rTerrainObj = Terrain.CreateTerrainGameObject(rTerrainData);
                    rTerrainObj.name = string.Format("tile_{0}_{1}", i, j);
                    rTerrainObj.transform.position = new Vector3(rTile.Offset.x + rOffset.x, 0, rTile.Offset.y + rOffset.y);
                    Terrain rTerrain = rTerrainObj.GetComponent<Terrain>();
                    rTerrain.materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
                    rTerrain.terrainData.alphamapResolution = mTileSize - 1;
                    rTerrain.terrainData.SetAlphamaps(0, 0, rAlphaMaps);
    
                    rTerrains.Add(rTerrain);
    
                    AssetDatabase.Refresh();
                    AssetDatabase.SaveAssets();
    
                    k++;
                }
            }
        }
    
        private bool IsTileValid(int rIndex, int rTileCount)
        {
            return rIndex >= 0 && rIndex < rTileCount;
        }
    
        public SplatPrototype CreateSplatPrototype(Texture2D tmpTexture, Vector2 tmpTileSize, Vector2 tmpOffset)
        {
            SplatPrototype outSplatPrototype = new SplatPrototype();
            outSplatPrototype.texture = tmpTexture;
            outSplatPrototype.tileOffset = tmpOffset;
            outSplatPrototype.tileSize = tmpTileSize;
            return outSplatPrototype;
        }
    
        public SplatPrototype[] CreateSplatPrototypes(SplatPrototype[] rBigSplatPrototypes, int rBaseTexRes, int rTileI, int rTileJ)
        {
            SplatPrototype[] outSplatPrototypes = new SplatPrototype[rBigSplatPrototypes.Length];
    
            for (int i = 0; i < rBigSplatPrototypes.Length; ++i)
            {
                Vector2 rTmpTileSize = rBigSplatPrototypes[i].tileSize;
                Vector2 rTmpOffset = new Vector2(rBaseTexRes / mTileNumWidth * rTileI, rBaseTexRes / mTileNumHeight * rTileJ);
                outSplatPrototypes[i] = CreateSplatPrototype(rBigSplatPrototypes[i].texture, rTmpTileSize, rTmpOffset);
            }
            return outSplatPrototypes;
        }
    }
}