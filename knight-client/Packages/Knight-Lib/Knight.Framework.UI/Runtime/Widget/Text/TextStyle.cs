using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [System.Serializable]
    public class TextStyle
    {
        public bool     IsEditorOpen;
        public string   Description;

        public int      ID;

        public Font     Font;

        public Color    Color;
        public int      FontSize;

        public bool     UseGradient;
        public Color    TopColor;
        public Color    BottomColor;

        public bool     UseOutline;
        public Color    OutlineColor;
        public Vector2  OutlineDistance;
        public bool     OutlineGraphicAlpha;

        public bool     UseShadow;
        public Color    ShadowColor;
        public Vector2  ShadowDistance;
        public bool     ShadowGraphicAlpha;
    }
}
