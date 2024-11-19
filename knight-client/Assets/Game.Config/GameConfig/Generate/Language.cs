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
    public partial class LanguageConfig
    {
        public string ID;
        public string ChineseSimplified;
        public string ChineseTraditional;
        public string English;
        public string Japanese;
        public string Indonesian;
        public string Korean;
        public string Malay;
        public string Thai;
        public string Arabic;
    }
}