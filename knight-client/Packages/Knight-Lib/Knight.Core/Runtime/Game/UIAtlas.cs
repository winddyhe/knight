using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Core
{
    public enum UIAtlasMode
    {
        Atlas,
        Icons,
        FullBG,
    }

    public class UIAtlas : ScriptableObject
    {
        public UIAtlasMode  Mode;
        public string       ABName;
        public List<string> Sprites;
    }
}
