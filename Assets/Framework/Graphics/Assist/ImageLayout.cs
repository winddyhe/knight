//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
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
        [Range(0, 1)]
        public float            FillAmount;
        public bool             IsClockwise;
        public int              FillOrigin;

        public VertexElement    VertexElement;

        public ImageLayout()
        {
            this.VertexElement = new VertexElement();
        }

        public void GenerateSprite(Sprite rSprite)
        {
            if (rSprite == null) return;

            switch (this.ImageType)
            {
                case Type.Simple:
                    this.GenerateSimpleSprite(rSprite);
                    break;
                case Type.Sliced:
                    this.GenerateSlicedSprite(rSprite);
                    break;
                case Type.Tiled:
                    this.GenerateTiledSprite(rSprite);
                    break;
                case Type.Filled:
                    this.GenerateFilledSprite(rSprite);
                    break;
                default:
                    this.GenerateSimpleSprite(rSprite);
                    break;
            }
        }

        public void SetMeshVertices(Mesh rMesh)
        {
            this.VertexElement.SetMeshVertices(rMesh);
        }

        public void SetMeshIndices(Mesh rMesh)
        {
            this.VertexElement.SetMeshIndices(rMesh);
        }

        private void GenerateSimpleSprite(Sprite rSprite)
        {
            var rUV = UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite);
            this.CalcAspect(rSprite, this.IsPreserveAspect);
            
            this.VertexElement.Clear();

            float rHalfWidth = this.Width / 2.0f;
            float rHalfHeight = this.Height / 2.0f;

            this.VertexElement.AddVert(new Vector3(0,     0     ), this.Color, new Vector2(rUV.x, rUV.y));
            this.VertexElement.AddVert(new Vector3(0,     Height), this.Color, new Vector2(rUV.x, rUV.w));
            this.VertexElement.AddVert(new Vector3(Width, Height), this.Color, new Vector2(rUV.z, rUV.w));
            this.VertexElement.AddVert(new Vector3(Width, 0     ), this.Color, new Vector2(rUV.z, rUV.y));
            
            this.VertexElement.AddTriangle(0, 1, 2);
            this.VertexElement.AddTriangle(2, 3, 0);
        }

        private static readonly Vector2[] s_Sliced_VertScratch = new Vector2[4];
        private static readonly Vector2[] s_Sliced_UVScratch   = new Vector2[4];

        private void GenerateSlicedSprite(Sprite rSprite)
        {
            bool bHasBorder = rSprite.border.sqrMagnitude > 0;

            if (!bHasBorder)
            {
                this.GenerateSimpleSprite(rSprite);
                return;
            }

            Vector4 rOuter, rInner, rPadding, rBorder;

            if (rSprite != null)
            {
                rOuter = UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite);
                rInner = UnityEngine.Sprites.DataUtility.GetInnerUV(rSprite);
                rPadding = UnityEngine.Sprites.DataUtility.GetPadding(rSprite);
                rBorder = rSprite.border;
            }
            else
            {
                rOuter = Vector4.zero;
                rInner = Vector4.zero;
                rPadding = Vector4.zero;
                rBorder = Vector4.zero;
            }

            Rect rRect = new Rect(0, 0, Width, Height);
            s_Sliced_VertScratch[0] = new Vector2(rPadding.x, rPadding.y);
            s_Sliced_VertScratch[3] = new Vector2(rRect.width - rPadding.z, rRect.height - rPadding.w);

            s_Sliced_VertScratch[1].x = rBorder.x;
            s_Sliced_VertScratch[1].y = rBorder.y;
            s_Sliced_VertScratch[2].x = rRect.width - rBorder.z;
            s_Sliced_VertScratch[2].y = rRect.height - rBorder.w;

            for (int i = 0; i < 4; ++i)
            {
                s_Sliced_VertScratch[i].x += rRect.x;
                s_Sliced_VertScratch[i].y += rRect.y;
            }

            s_Sliced_UVScratch[0] = new Vector2(rOuter.x, rOuter.y);
            s_Sliced_UVScratch[1] = new Vector2(rInner.x, rInner.y);
            s_Sliced_UVScratch[2] = new Vector2(rInner.z, rInner.w);
            s_Sliced_UVScratch[3] = new Vector2(rOuter.z, rOuter.w);

            this.VertexElement.Clear();

            for (int x = 0; x < 3; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < 3; ++y)
                {
                    if (!IsFillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;

                    this.AddQuad(this.VertexElement,
                        new Vector2(s_Sliced_VertScratch[x].x, s_Sliced_VertScratch[y].y),
                        new Vector2(s_Sliced_VertScratch[x2].x, s_Sliced_VertScratch[y2].y),
                        this.Color,
                        new Vector2(s_Sliced_UVScratch[x].x, s_Sliced_UVScratch[y].y),
                        new Vector2(s_Sliced_UVScratch[x2].x, s_Sliced_UVScratch[y2].y));
                }
            }
        }

        private void GenerateTiledSprite(Sprite rSprite)
        {
            Vector4 rOuter, rInner, rBorder;
            Vector2 spriteSize;

            bool bHasBorder = rSprite.border.sqrMagnitude > 0;

            if (rSprite != null)
            {
                rOuter = UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite);
                rInner = UnityEngine.Sprites.DataUtility.GetInnerUV(rSprite);
                rBorder = rSprite.border;
                spriteSize = rSprite.rect.size;
            }
            else
            {
                rOuter = Vector4.zero;
                rInner = Vector4.zero;
                rBorder = Vector4.zero;
                spriteSize = Vector2.one * 100;
            }

            Rect rRect = new Rect(0, 0, Width, Height);
            float fTileWidth = (spriteSize.x - rBorder.x - rBorder.z);
            float fTileHeight = (spriteSize.y - rBorder.y - rBorder.w);

            var rUvMin = new Vector2(rInner.x, rInner.y);
            var rUvMax = new Vector2(rInner.z, rInner.w);
            
            // Min to max max range for tiled region in coordinates relative to lower left corner.
            float fXMin = rBorder.x;
            float fXMax = rRect.width - rBorder.z;
            float fYMin = rBorder.y;
            float fYMax = rRect.height - rBorder.w;

            this.VertexElement.Clear();
            var rClipped = rUvMax;

            // if either with is zero we cant tile so just assume it was the full width.
            if (fTileWidth == 0)
                fTileWidth = fXMax - fXMin;

            if (fTileHeight == 0)
                fTileHeight = fYMax - fYMin;

            if (this.IsFillCenter)
            {
                for (float y1 = fYMin; y1 < fYMax; y1 += fTileHeight)
                {
                    float y2 = y1 + fTileHeight;
                    if (y2 > fYMax)
                    {
                        rClipped.y = rUvMin.y + (rUvMax.y - rUvMin.y) * (fYMax - y1) / (y2 - y1);
                        y2 = fYMax;
                    }

                    rClipped.x = rUvMax.x;
                    for (float x1 = fXMin; x1 < fXMax; x1 += fTileWidth)
                    {
                        float x2 = x1 + fTileWidth;
                        if (x2 > fXMax)
                        {
                            rClipped.x = rUvMin.x + (rUvMax.x - rUvMin.x) * (fXMax - x1) / (x2 - x1);
                            x2 = fXMax;
                        }
                        AddQuad(this.VertexElement, new Vector2(x1, y1) + rRect.position, new Vector2(x2, y2) + rRect.position, this.Color, rUvMin, rClipped);
                    }
                }
            }

            if (bHasBorder)
            {
                rClipped = rUvMax;
                for (float y1 = fYMin; y1 < fYMax; y1 += fTileHeight)
                {
                    float y2 = y1 + fTileHeight;
                    if (y2 > fYMax)
                    {
                        rClipped.y = rUvMin.y + (rUvMax.y - rUvMin.y) * (fYMax - y1) / (y2 - y1);
                        y2 = fYMax;
                    }
                    AddQuad(VertexElement,
                        new Vector2(0, y1) + rRect.position,
                        new Vector2(fXMin, y2) + rRect.position,
                        this.Color,
                        new Vector2(rOuter.x, rUvMin.y),
                        new Vector2(rUvMin.x, rClipped.y));
                    AddQuad(VertexElement,
                        new Vector2(fXMax, y1) + rRect.position,
                        new Vector2(rRect.width, y2) + rRect.position,
                        this.Color,
                        new Vector2(rUvMax.x, rUvMin.y),
                        new Vector2(rOuter.z, rClipped.y));
                }

                // Bottom and top tiled border
                rClipped = rUvMax;
                for (float x1 = fXMin; x1 < fXMax; x1 += fTileWidth)
                {
                    float x2 = x1 + fTileWidth;
                    if (x2 > fXMax)
                    {
                        rClipped.x = rUvMin.x + (rUvMax.x - rUvMin.x) * (fXMax - x1) / (x2 - x1);
                        x2 = fXMax;
                    }
                    AddQuad(VertexElement,
                        new Vector2(x1, 0) + rRect.position,
                        new Vector2(x2, fYMin) + rRect.position,
                        this.Color,
                        new Vector2(rUvMin.x, rOuter.y),
                        new Vector2(rClipped.x, rUvMin.y));
                    AddQuad(VertexElement,
                        new Vector2(x1, fYMax) + rRect.position,
                        new Vector2(x2, rRect.height) + rRect.position,
                        this.Color,
                        new Vector2(rUvMin.x, rUvMax.y),
                        new Vector2(rClipped.x, rOuter.w));
                }

                // Corners
                AddQuad(VertexElement,
                    new Vector2(0, 0) + rRect.position,
                    new Vector2(fXMin, fYMin) + rRect.position,
                    this.Color,
                    new Vector2(rOuter.x, rOuter.y),
                    new Vector2(rUvMin.x, rUvMin.y));
                AddQuad(VertexElement,
                    new Vector2(fXMax, 0) + rRect.position,
                    new Vector2(rRect.width, fYMin) + rRect.position,
                    this.Color,
                    new Vector2(rUvMax.x, rOuter.y),
                    new Vector2(rOuter.z, rUvMin.y));
                AddQuad(VertexElement,
                    new Vector2(0, fYMax) + rRect.position,
                    new Vector2(fXMin, rRect.height) + rRect.position,
                    this.Color,
                    new Vector2(rOuter.x, rUvMax.y),
                    new Vector2(rUvMin.x, rOuter.w));
                AddQuad(VertexElement,
                    new Vector2(fXMax, fYMax) + rRect.position,
                    new Vector2(rRect.width, rRect.height) + rRect.position,
                    this.Color,
                    new Vector2(rUvMax.x, rUvMax.y),
                    new Vector2(rOuter.z, rOuter.w));
            }
        }

        private static readonly Vector3[] s_Filled_Xy = new Vector3[4];
        private static readonly Vector3[] s_Filled_Uv = new Vector3[4];

        void GenerateFilledSprite(Sprite rSprite)
        {
            this.VertexElement.Clear();

            if (this.FillAmount < 0.001f)
                return;

            CalcAspect(rSprite, IsPreserveAspect);

            Vector4 rRect = new Vector4(0, 0, Width, Height);
            Vector4 rOuter = rSprite != null ? UnityEngine.Sprites.DataUtility.GetOuterUV(rSprite) : Vector4.zero;

            float tx0 = rOuter.x;
            float ty0 = rOuter.y;
            float tx1 = rOuter.z;
            float ty1 = rOuter.w;

            // Horizontal and vertical filled sprites are simple -- just end the Image prematurely
            if (this.ImageFillMethod == FillMethod.Horizontal || this.ImageFillMethod == FillMethod.Vertical)
            {
                if (this.ImageFillMethod == FillMethod.Horizontal)
                {
                    float fFill = (tx1 - tx0) * this.FillAmount;

                    if (this.FillOrigin == 1)
                    {
                        rRect.x = rRect.z - (rRect.z - rRect.x) * this.FillAmount;
                        tx0 = tx1 - fFill;
                    }
                    else
                    {
                        rRect.z = rRect.x + (rRect.z - rRect.x) * this.FillAmount;
                        tx1 = tx0 + fFill;
                    }
                }
                else if (this.ImageFillMethod == FillMethod.Vertical)
                {
                    float fFill = (ty1 - ty0) * this.FillAmount;

                    if (this.FillOrigin == 1)
                    {
                        rRect.y = rRect.w - (rRect.w - rRect.y) * this.FillAmount;
                        ty0 = ty1 - fFill;
                    }
                    else
                    {
                        rRect.w = rRect.y + (rRect.w - rRect.y) * this.FillAmount;
                        ty1 = ty0 + fFill;
                    }
                }
            }

            s_Filled_Xy[0] = new Vector2(rRect.x, rRect.y);
            s_Filled_Xy[1] = new Vector2(rRect.x, rRect.w);
            s_Filled_Xy[2] = new Vector2(rRect.z, rRect.w);
            s_Filled_Xy[3] = new Vector2(rRect.z, rRect.y);

            s_Filled_Uv[0] = new Vector2(tx0, ty0);
            s_Filled_Uv[1] = new Vector2(tx0, ty1);
            s_Filled_Uv[2] = new Vector2(tx1, ty1);
            s_Filled_Uv[3] = new Vector2(tx1, ty0);

            {
                if (this.FillAmount < 1f && this.ImageFillMethod != FillMethod.Horizontal && this.ImageFillMethod != FillMethod.Vertical)
                {
                    if (this.ImageFillMethod == FillMethod.Radial90)
                    {
                        if (RadialCut(s_Filled_Xy, s_Filled_Uv, this.FillAmount, this.IsClockwise, this.FillOrigin))
                            AddQuad(this.VertexElement, s_Filled_Xy, this.Color, s_Filled_Uv);
                    }
                    else if (this.ImageFillMethod == FillMethod.Radial180)
                    {
                        for (int side = 0; side < 2; ++side)
                        {
                            float fx0, fx1, fy0, fy1;
                            int even = this.FillOrigin > 1 ? 1 : 0;

                            if (this.FillOrigin == 0 || this.FillOrigin == 2)
                            {
                                fy0 = 0f;
                                fy1 = 1f;
                                if (side == even)
                                {
                                    fx0 = 0f;
                                    fx1 = 0.5f;
                                }
                                else
                                {
                                    fx0 = 0.5f;
                                    fx1 = 1f;
                                }
                            }
                            else
                            {
                                fx0 = 0f;
                                fx1 = 1f;
                                if (side == even)
                                {
                                    fy0 = 0.5f;
                                    fy1 = 1f;
                                }
                                else
                                {
                                    fy0 = 0f;
                                    fy1 = 0.5f;
                                }
                            }

                            s_Filled_Xy[0].x = Mathf.Lerp(rRect.x, rRect.z, fx0);
                            s_Filled_Xy[1].x = s_Filled_Xy[0].x;
                            s_Filled_Xy[2].x = Mathf.Lerp(rRect.x, rRect.z, fx1);
                            s_Filled_Xy[3].x = s_Filled_Xy[2].x;

                            s_Filled_Xy[0].y = Mathf.Lerp(rRect.y, rRect.w, fy0);
                            s_Filled_Xy[1].y = Mathf.Lerp(rRect.y, rRect.w, fy1);
                            s_Filled_Xy[2].y = s_Filled_Xy[1].y;
                            s_Filled_Xy[3].y = s_Filled_Xy[0].y;

                            s_Filled_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Filled_Uv[1].x = s_Filled_Uv[0].x;
                            s_Filled_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Filled_Uv[3].x = s_Filled_Uv[2].x;

                            s_Filled_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Filled_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Filled_Uv[2].y = s_Filled_Uv[1].y;
                            s_Filled_Uv[3].y = s_Filled_Uv[0].y;

                            float val = this.IsClockwise ? this.FillAmount * 2f - side : this.FillAmount * 2f - (1 - side);

                            if (RadialCut(s_Filled_Xy, s_Filled_Uv, Mathf.Clamp01(val), this.IsClockwise, ((side + this.FillOrigin + 3) % 4)))
                            {
                                AddQuad(this.VertexElement, s_Filled_Xy, this.Color, s_Filled_Uv);
                            }
                        }
                    }
                    else if (this.ImageFillMethod == FillMethod.Radial360)
                    {
                        for (int corner = 0; corner < 4; ++corner)
                        {
                            float fx0, fx1, fy0, fy1;

                            if (corner < 2)
                            {
                                fx0 = 0f;
                                fx1 = 0.5f;
                            }
                            else
                            {
                                fx0 = 0.5f;
                                fx1 = 1f;
                            }

                            if (corner == 0 || corner == 3)
                            {
                                fy0 = 0f;
                                fy1 = 0.5f;
                            }
                            else
                            {
                                fy0 = 0.5f;
                                fy1 = 1f;
                            }

                            s_Filled_Xy[0].x = Mathf.Lerp(rRect.x, rRect.z, fx0);
                            s_Filled_Xy[1].x = s_Filled_Xy[0].x;
                            s_Filled_Xy[2].x = Mathf.Lerp(rRect.x, rRect.z, fx1);
                            s_Filled_Xy[3].x = s_Filled_Xy[2].x;

                            s_Filled_Xy[0].y = Mathf.Lerp(rRect.y, rRect.w, fy0);
                            s_Filled_Xy[1].y = Mathf.Lerp(rRect.y, rRect.w, fy1);
                            s_Filled_Xy[2].y = s_Filled_Xy[1].y;
                            s_Filled_Xy[3].y = s_Filled_Xy[0].y;

                            s_Filled_Uv[0].x = Mathf.Lerp(tx0, tx1, fx0);
                            s_Filled_Uv[1].x = s_Filled_Uv[0].x;
                            s_Filled_Uv[2].x = Mathf.Lerp(tx0, tx1, fx1);
                            s_Filled_Uv[3].x = s_Filled_Uv[2].x;

                            s_Filled_Uv[0].y = Mathf.Lerp(ty0, ty1, fy0);
                            s_Filled_Uv[1].y = Mathf.Lerp(ty0, ty1, fy1);
                            s_Filled_Uv[2].y = s_Filled_Uv[1].y;
                            s_Filled_Uv[3].y = s_Filled_Uv[0].y;

                            float val = this.IsClockwise ?
                                this.FillAmount * 4f - ((corner + this.FillOrigin) % 4) :
                                this.FillAmount * 4f - (3 - ((corner + this.FillOrigin) % 4));

                            if (RadialCut(s_Filled_Xy, s_Filled_Uv, Mathf.Clamp01(val), this.IsClockwise, ((corner + 2) % 4)))
                                AddQuad(this.VertexElement, s_Filled_Xy, this.Color, s_Filled_Uv);
                        }
                    }
                }
                else
                {
                    AddQuad(this.VertexElement, s_Filled_Xy, this.Color, s_Filled_Uv);
                }
            }
        }

        private void AddQuad(VertexElement rVertexElement, Vector2 rPosMin, Vector2 rPosMax, Color32 rColor, Vector2 rUvMin, Vector2 rUvMax)
        {
            int nStartIndex = rVertexElement.Vertices.Count;

            rVertexElement.AddVert(new Vector3(rPosMin.x, rPosMin.y, 0), rColor, new Vector2(rUvMin.x, rUvMin.y));
            rVertexElement.AddVert(new Vector3(rPosMin.x, rPosMax.y, 0), rColor, new Vector2(rUvMin.x, rUvMax.y));
            rVertexElement.AddVert(new Vector3(rPosMax.x, rPosMax.y, 0), rColor, new Vector2(rUvMax.x, rUvMax.y));
            rVertexElement.AddVert(new Vector3(rPosMax.x, rPosMin.y, 0), rColor, new Vector2(rUvMax.x, rUvMin.y));

            rVertexElement.AddTriangle(nStartIndex, nStartIndex + 1, nStartIndex + 2);
            rVertexElement.AddTriangle(nStartIndex + 2, nStartIndex + 3, nStartIndex);
        }

        private void AddQuad(VertexElement rVertexElement, Vector3[] rQuadPositions, Color32 rColor, Vector3[] rQuadUVs)
        {
            int startIndex = rVertexElement.Vertices.Count;

            for (int i = 0; i < 4; ++i)
                rVertexElement.AddVert(rQuadPositions[i], rColor, rQuadUVs[i]);

            rVertexElement.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            rVertexElement.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        private void CalcAspect(Sprite rSprite, bool bShouldPreserveAspect)
        {
            float fSqrMagnitude = rSprite.rect.width * rSprite.rect.width + rSprite.rect.height * rSprite.rect.height;

            if (bShouldPreserveAspect && fSqrMagnitude > 0.0f)
            {
                var fSpriteRatio = rSprite.rect.width / rSprite.rect.height;
                var fRectRatio = Width / Height;

                if (fSpriteRatio > fRectRatio)
                {
                    var fOldHeight = Height;
                    this.Height = this.Width * (1.0f / fSpriteRatio);
                }
                else
                {
                    var fOldWidth = this.Width;
                    this.Width = this.Height * fSpriteRatio;
                }
            }
        }

        private bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            // Nothing to fill
            if (fill < 0.001f) return false;

            // Even corners invert the fill direction
            if ((corner & 1) == 1) invert = !invert;

            // Nothing to adjust
            if (!invert && fill > 0.999f) return true;

            // Convert 0-1 value into 0 to 90 degrees angle in radians
            float angle = Mathf.Clamp01(fill);
            if (invert) angle = 1f - angle;
            angle *= 90f * Mathf.Deg2Rad;

            // Calculate the effective X and Y factors
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);

            RadialCut(xy, cos, sin, invert, corner);
            RadialCut(uv, cos, sin, invert, corner);
            return true;
        }

        /// <summary>
        /// Adjust the specified quad, making it be radially filled instead.
        /// </summary>

        private void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int i0 = corner;
            int i1 = ((corner + 1) % 4);
            int i2 = ((corner + 2) % 4);
            int i3 = ((corner + 3) % 4);

            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i2].x = xy[i1].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i2].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i3].y = xy[i2].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (!invert) xy[i3].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                else xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;

                    if (!invert)
                    {
                        xy[i1].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                        xy[i2].y = xy[i1].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;

                    if (invert)
                    {
                        xy[i2].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
                        xy[i3].x = xy[i2].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }

                if (invert) xy[i3].y = Mathf.Lerp(xy[i0].y, xy[i2].y, sin);
                else xy[i1].x = Mathf.Lerp(xy[i0].x, xy[i2].x, cos);
            }
        }
    }
}
