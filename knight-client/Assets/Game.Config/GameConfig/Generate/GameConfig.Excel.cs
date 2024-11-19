using System;
using System.IO;
using System.Collections.Generic;
using Knight.Core;
using Knight.Framework.Serializer;

namespace Game
{
    [SerializerBinary]
    [SBGroup("GameConfig")]
    /// <summary>
    /// Auto generate code, don't modify it.
    /// </summary>
    public partial class GameConfig
    {
        [JsonPath("Language")]
        public LanguageConfigTable Language;
        [JsonPath("Skill")]
        public SkillConfigTable Skill;
        [JsonPath("SkillAsset")]
        public SkillAssetConfigTable SkillAsset;
        [JsonPath("Stage")]
        public StageConfigTable Stage;
        [JsonPath("Unit")]
        public UnitConfigTable Unit;
    }
}