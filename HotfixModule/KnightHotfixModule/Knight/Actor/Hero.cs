//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================

using WindHotfix.Core;

namespace KnightHotfixModule.Knight
{
    [HotfixSBGroup("GameConfig")]
    public partial class Hero : HotfixSerializerBinary
    {
        public int      ID;
        public string   Name;
        public int      AvatarID;
        public int      SkillID;
        public float    Scale;
        public float    Height;
        public float    Radius;
    }
}