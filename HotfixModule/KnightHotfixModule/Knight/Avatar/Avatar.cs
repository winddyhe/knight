//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using WindHotfix.Core;

namespace KnightHotfixModule.Knight
{
    /// <summary>
    /// 角色的Avatar配置
    /// </summary>
    [HotfixSBGroup("GameConfig")]
    public partial class Avatar : HotfixSerializerBinary
    {
        public int      ID;
        public string   AvatarName;
        public string   ABPath;
        public string   AssetName;
    }
}
