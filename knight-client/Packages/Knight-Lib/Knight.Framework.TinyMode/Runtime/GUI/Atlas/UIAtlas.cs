//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{
    public enum UIAtlasMode
    {
        Atlas,
        Icons,
        FullBG,
    }

    public class UIAtlas : ScriptableObject
    {
        public UIAtlasMode      Mode;
        public string           ABName;

        public List<Sprite>     Sprites;
        public List<Texture2D>  Textures;
    }
}
