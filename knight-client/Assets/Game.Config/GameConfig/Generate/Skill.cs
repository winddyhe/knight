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
    public partial class SkillConfig
    {
        public int ID;
        public string SkillName;
        public int SkillCastTargetType;
        public int TargetSelectCampType;
        public int TargetSelectSearchType;
        public int TargetSelectRadiusOrHeight;
        public int TargetSelectAngleOrWidth;
    }
}