//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.Editor;
using UnityEditor;
using UnityEditor.AssetBundles;

namespace Game.Knight.Editor
{
    public class ABEntryProcessor_GameConfig : ABEntryProcessor
    {
        public override void PreprocessAssets()
        {
            //GameConfig.Instance.Load_Local(this.Entry.abOriginalResPath);
            AssetDatabase.Refresh();
        }
    }

    public class ABEntryProcessor_SkillConfig : ABEntryProcessor
    {
        public override void PreprocessAssets()
        {
            //GPCSkillConfig.Instance.Load_Local(this.Entry.abOriginalResPath);
            AssetDatabase.Refresh();
        }
    }
}