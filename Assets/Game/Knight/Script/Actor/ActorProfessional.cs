//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Core.Serializer;

namespace Game.Knight
{
    [SBGroup("GameConfig")]
    public partial class ActorProfessional : SerializerBinary
    {
        public int      ID;
        public int      HeroID;
        public string   Name;
        public string   Desc;
    }
}
