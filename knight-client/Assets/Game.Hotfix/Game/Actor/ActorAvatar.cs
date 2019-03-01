//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Hotfix.Core;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// 角色的Avatar配置
    /// </summary>
    [HotfixSBGroup("GameConfig")]
    public partial class ActorAvatar : HotfixSerializerBinary
    {
        public int      ID;
        public string   AvatarName;
        public string   ABPath;
        public string   AssetName;
    }
}
