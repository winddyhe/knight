//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Framework;
using Core.Serializer;

namespace Game.Knight
{
    /// <summary>
    /// 角色的Avatar配置
    /// </summary>
    [SBGroup("GameConfig")]
    public partial class Avatar : SerializerBinary
    {
        public int      ID;
        public string   AvatarName;
        public string   ABPath;
        public string   AssetName;
    }
}
