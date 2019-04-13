using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace Knight.Framework.TinyMode.UI.Editor
{
    public class UIAtlasTools
    {
        [MenuItem("Tools/GUI/Atlas config generate")]
        public static void GenerateAtlas()
        {
            string rTextureRoot = "Assets/Game/UIAssets/Textures";
            string rAtlasRoot   = rTextureRoot + "/Atlas";
            string rConfigRoot  = "Assets/Game/Resources/GUI" + "/Config";
            string rIconsRoot   = rTextureRoot + "/Icons";
            string rFullBGRoot  = rTextureRoot + "/FullBG";

            // 生成所有的Atlas
            DirectoryInfo rDirInfo = new DirectoryInfo(rAtlasRoot);
            var rAllDirs = rDirInfo.GetDirectories();
            for (int i = 0; i < rAllDirs.Length; i++)
            {
                var rUIAtlas = GetAtlas(rConfigRoot + "/config_" + rAllDirs[i].Name + ".asset");
                rUIAtlas.Mode = UIAtlasMode.Atlas;
                rUIAtlas.ABName = "GUI/Textures/Atlas/" + rAllDirs[i].Name;

                string rAtlasPath = rAtlasRoot + "/" + rAllDirs[i].Name;
                rUIAtlas.Sprites = new List<Sprite>();
                string[] rGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rAtlasPath });
                for (int j = 0; j < rGUIDs.Length; j++)
                {
                    string rTexPath = AssetDatabase.GUIDToAssetPath(rGUIDs[j]);
                    rUIAtlas.Sprites.Add(AssetDatabase.LoadAssetAtPath(rTexPath, typeof(Sprite)) as Sprite);
                }
                EditorUtility.SetDirty(rUIAtlas);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 生成FullBG
            var rFullBGUIAtlas = GetAtlas(rConfigRoot + "/config_fullbg.asset");
            rFullBGUIAtlas.ABName = "GUI/Textures/FullBg";
            rFullBGUIAtlas.Mode = UIAtlasMode.FullBG;
            rFullBGUIAtlas.Sprites = new List<Sprite>();
            string[] rFullBGGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rFullBGRoot });
            for (int i = 0; i < rFullBGGUIDs.Length; i++)
            {
                string rTexPath = AssetDatabase.GUIDToAssetPath(rFullBGGUIDs[i]);
                rFullBGUIAtlas.Textures.Add(AssetDatabase.LoadAssetAtPath(rTexPath, typeof(Texture2D)) as Texture2D);
            }
            EditorUtility.SetDirty(rFullBGUIAtlas);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 生成icons
            var rIconsUIAtlas = GetAtlas(rConfigRoot + "/config_icons.asset");
            rIconsUIAtlas.ABName = "GUI/Textures/Icons";
            rIconsUIAtlas.Mode = UIAtlasMode.Icons;
            rIconsUIAtlas.Sprites = new List<Sprite>();
            string[] rIconsGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rIconsRoot });
            for (int i = 0; i < rIconsGUIDs.Length; i++)
            {
                string rTexPath = AssetDatabase.GUIDToAssetPath(rIconsGUIDs[i]);
                rIconsUIAtlas.Textures.Add(AssetDatabase.LoadAssetAtPath(rTexPath, typeof(Texture2D)) as Texture2D);
            }
            EditorUtility.SetDirty(rIconsUIAtlas);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Generate atlas config success.");
        }

        private static UIAtlas GetAtlas(string rPath)
        {
            var rAtlas = AssetDatabase.LoadAssetAtPath(rPath, typeof(UIAtlas)) as UIAtlas;
            if (rAtlas == null)
            {
                rAtlas = ScriptableObject.CreateInstance<UIAtlas>();
                AssetDatabase.CreateAsset(rAtlas, rPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                rAtlas = AssetDatabase.LoadAssetAtPath(rPath, typeof(UIAtlas)) as UIAtlas;
            }                
            return rAtlas;
        }
    }
}