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
    public partial class SkillAssetConfig
    {
        public string ID;
        public string SkillEffectName;
        public string SkillTimelineName;
        public string SkillScriptName;
    }
}