using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

namespace UnityEditor.UI
{
    public class UIAtlasTools
    {
        [MenuItem("Tools/GUI/Atlas config generate")]
        public static void GenerateAtlas()
        {
            string rTextureRoot = "Assets/Game/GameAsset/GUI/Textures";
            string rAtlasRoot   = rTextureRoot + "/Atlas";
            string rConfigRoot  = rTextureRoot + "/Config";
            //string rIconsRoot   = rTextureRoot + "/Icons";
            //string rFullBGRoot  = rTextureRoot + "/FullBG";

            // 生成所有的Atlas
            DirectoryInfo rDirInfo = new DirectoryInfo(rAtlasRoot);
            var rAllDirs = rDirInfo.GetDirectories();
            for (int i = 0; i < rAllDirs.Length; i++)
            {
                var rUIAtlas = GetAtlas(rConfigRoot + "/config_" + rAllDirs[i].Name + ".asset");
                rUIAtlas.Mode = UIAtlasMode.Atlas;
                rUIAtlas.ABName = "game/ui/atlas/" + rAllDirs[i].Name + ".ab";

                string rAtlasPath = rAtlasRoot + "/" + rAllDirs[i].Name;
                rUIAtlas.Sprites = new List<string>();
                string[] rGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rAtlasPath });
                for (int j = 0; j < rGUIDs.Length; j++)
                {
                    string rTexPath = AssetDatabase.GUIDToAssetPath(rGUIDs[j]);
                    rUIAtlas.Sprites.Add(Path.GetFileNameWithoutExtension(rTexPath));
                }
                EditorUtility.SetDirty(rUIAtlas);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // 生成FullBG
            //var rFullBGUIAtlas = GetAtlas(rConfigRoot + "/config_fullbg.asset");
            //rFullBGUIAtlas.ABName = "game/ui/fullbg";
            //rFullBGUIAtlas.Mode = UIAtlasMode.FullBG;
            //rFullBGUIAtlas.Sprites = new List<string>();
            //string[] rFullBGGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rFullBGRoot });
            //for (int i = 0; i < rFullBGGUIDs.Length; i++)
            //{
            //    string rTexPath = AssetDatabase.GUIDToAssetPath(rFullBGGUIDs[i]);
            //    rFullBGUIAtlas.Sprites.Add(Path.GetFileNameWithoutExtension(rTexPath));
            //}
            //EditorUtility.SetDirty(rFullBGUIAtlas);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();

            // 生成icons
            //var rIconsUIAtlas = GetAtlas(rConfigRoot + "/config_icons.asset");
            //rIconsUIAtlas.ABName = "game/ui/icons.ab";
            //rIconsUIAtlas.Mode = UIAtlasMode.Icons;
            //rIconsUIAtlas.Sprites = new List<string>();
            //string[] rIconsGUIDs = AssetDatabase.FindAssets("t:Texture", new string[] { rIconsRoot });
            //for (int i = 0; i < rIconsGUIDs.Length; i++)
            //{
            //    string rTexPath = AssetDatabase.GUIDToAssetPath(rIconsGUIDs[i]);
            //    rIconsUIAtlas.Sprites.Add(Path.GetFileNameWithoutExtension(rTexPath));
            //}
            //EditorUtility.SetDirty(rIconsUIAtlas);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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