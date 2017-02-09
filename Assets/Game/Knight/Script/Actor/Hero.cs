//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework;
using Core.Serializer;

namespace Game.Knight
{
    [SBGroup("GameConfig")]
    public partial class Hero : SerializerBinary
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