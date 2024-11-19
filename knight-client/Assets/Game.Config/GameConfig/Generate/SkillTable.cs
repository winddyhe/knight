using System;
using System.IO;
using Knight.Framework.Serializer;
using System.Collections.Generic;

namespace Game
{
    [SerializerBinary]
    [SBGroup("GameConfig")]
    [SBFileReadWrite]
    /// <summary>
    /// Auto generate code, don't modify it.
    /// </summary>
    public partial class SkillConfigTable
    {
        public Dictionary<System.Int32, SkillConfig> Table;
    }
}