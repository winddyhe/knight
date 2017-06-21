using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphics
{
    [System.Serializable]
    public class ImageLayout
    {
        public enum Type
        {
            Simple,
            Sliced,
            Tiled,
            Filled
        }

        public enum FillMethod
        {
            Horizontal,
            Vertical,
            Radial90,
            Radial180,
            Radial360,
        }

        public enum OriginHorizontal
        {
            Left,
            Right,
        }

        public enum OriginVertical
        {
            Bottom,
            Top,
        }

        public enum Origin90
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight,
        }

        public enum Origin180
        {
            Bottom,
            Left,
            Top,
            Right,
        }

        public enum Origin360
        {
            Bottom,
            Right,
            Top,
            Left,
        }

        public float            Width;
        public float            Height;
        public Color            Color;

        public Type             ImageType;
        public FillMethod       ImageFillMethod;

        public OriginHorizontal ImageOriginHorizontal;
        public OriginVertical   ImageOriginVertical;
        public Origin90         ImageOrigin90;
        public Origin180        ImageOrigin180;
        public Origin360        ImageOrigin360;

        public bool             IsPreserveAspect;
        public bool             IsFillCenter;
        public float            FillAmount;
        public bool             IsClockwise;

        public VertexElement    VertexElement;

        public ImageLayout()
        {
            this.VertexElement = new VertexElement();
        }
        
        private void GenerateSimpleSprite(Sprite rSprite)
        {
            var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite);

            float fHalfWidth = Width / 2.0f;
            float fHalfHeight = Height / 2.0f;

            this.VertexElement.Clear();
            
            this.VertexElement.AddVert(new Vector3(-fHalfWidth, -fHalfHeight, 0), this.Color, new Vector2(uv.x, uv.y));
            this.VertexElement.AddVert(new Vector3( fHalfWidth, -fHalfHeight, 0), this.Color, new Vector2(uv.x, uv.w));
            this.VertexElement.AddVert(new Vector3( fHalfWidth,  fHalfHeight, 0), this.Color, new Vector2(uv.z, uv.w));
            this.VertexElement.AddVert(new Vector3(-fHalfWidth,  fHalfHeight, 0), this.Color, new Vector2(uv.z, uv.y));
            
            this.VertexElement.AddTriangle(0, 1, 2);
            this.VertexElement.AddTriangle(2, 3, 0);
        }
    }
}
