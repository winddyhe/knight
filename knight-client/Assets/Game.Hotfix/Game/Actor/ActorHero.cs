//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================

using Knight.Hotfix.Core;

namespace Game
{
    [HotfixSBGroup("GameConfig")]
    public partial class ActorHero : HotfixSerializerBinary
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