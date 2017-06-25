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

        public void GenerateSprite(Sprite rSprite)
        {
            switch (this.ImageType)
            {
                case Type.Simple:
                    break;
                case Type.Sliced:
                    break;
                case Type.Tiled:
                    break;
                case Type.Filled:
                    break;
                default:
                    break;
            }
            this.GenerateSimpleSprite(rSprite);
        }
        
        public void GenerateSimpleSprite(Sprite rSprite)
        {
            var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite);
            var vert = GetDrawingDimensions(rSprite, this.IsPreserveAspect);
            
            this.VertexElement.Clear();
            
            this.VertexElement.AddVert(new Vector3(vert.x, vert.y), this.Color, new Vector2(uv.x, uv.y));
            this.VertexElement.AddVert(new Vector3(vert.x, vert.w), this.Color, new Vector2(uv.x, uv.w));
            this.VertexElement.AddVert(new Vector3(vert.z, vert.w), this.Color, new Vector2(uv.z, uv.w));
            this.VertexElement.AddVert(new Vector3(vert.z, vert.y), this.Color, new Vector2(uv.z, uv.y));
            
            this.VertexElement.AddTriangle(0, 1, 2);
            this.VertexElement.AddTriangle(2, 3, 0);
        }

        public void SetMeshVertices(Mesh rMesh)
        {
            this.VertexElement.SetMeshVertices(rMesh);
        }

        public void SetMeshIndices(Mesh rMesh)
        {
            this.VertexElement.SetMeshIndices(rMesh);
        }

        private Vector4 GetDrawingDimensions(Sprite rSprite, bool bShouldPreserveAspect)
        {
            var padding = UnityEngine.Sprites.DataUtility.GetPadding(rSprite);
            var size = new Vector2(rSprite.rect.width, rSprite.rect.height);

            Rect r = new Rect(0, 0, Width, Height);

            int spriteW = Mathf.RoundToInt(size.x);
            int spriteH = Mathf.RoundToInt(size.y);

            var v = new Vector4(
                    padding.x / spriteW,
                    padding.y / spriteH,
                    (spriteW - padding.z) / spriteW,
                    (spriteH - padding.w) / spriteH);

            if (bShouldPreserveAspect && size.sqrMagnitude > 0.0f)
            {
                var spriteRatio = size.x / size.y;
                var rectRatio = r.width / r.height;

                if (spriteRatio > rectRatio)
                {
                    var oldHeight = r.height;
                    r.height = r.width * (1.0f / spriteRatio);
                    r.y += (oldHeight - r.height);
                }
                else
                {
                    var oldWidth = r.width;
                    r.width = r.height * spriteRatio;
                    r.x += (oldWidth - r.width);
                }
            }

            v = new Vector4(
                    r.x + r.width * v.x,
                    r.y + r.height * v.y,
                    r.x + r.width * v.z,
                    r.y + r.height * v.w
                    );

            return v;
        }
    }
}
