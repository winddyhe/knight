using System;
using System.IO;
using Knight.Framework.Serializer;
using System.Collections.Generic;
using Knight.Core;

namespace Game
{
    [SerializerBinary]
    [SBGroup("GameConfig")]
    /// <summary>
    /// Auto generate code, don't modify it.
    /// </summary>
    public partial class UnitConfig
    {
        public int ID;
        public int ActorType;
        public string Name;
        public string PrefabABPath;
        public int ModelScale;
        public int NormalSkillIntervalTime;
        public int[] NormalSkills;
        public int Skill1;
        public int Skill2;
        public int Skill3;
        public int Skill4;
    }
}