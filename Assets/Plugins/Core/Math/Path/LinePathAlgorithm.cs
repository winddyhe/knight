//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Math
{
    public class LinePathAlgorithm
    {
        /// <summary>
        /// 线的初始数据
        /// </summary>
        [System.Serializable]
        public class Line
        {
            public Vector3          Up;
            public Vector3          Start;
            public Vector3          End;

            public float            Width;
            public float            Height;
        }

        /// <summary>
        /// 由Line生成的LineBox
        /// </summary>
        public class LineBox
        {
            /// <summary>
            /// 线初始数据
            /// </summary>
            public Line             Line;
            /// <summary>
            /// OBB
            /// </summary>
            public OBB              OBB;
            /// <summary>
            /// 竖直的平面
            /// </summary>
            public Plane            VerticalPlane;

            public LineBox(Line rLine)
            {
                this.Line = rLine;
                this.CreateOBB(rLine);
            }

            private void CreateOBB(Line rLine)
            {
                Vector3 rCenter = (rLine.Start + rLine.End) * 0.5f + rLine.Up * rLine.Height * 0.5f;
                Vector3 rExtends = new Vector3((rLine.End - rLine.Start).magnitude * 0.5f, rLine.Height * 0.5f, rLine.Width * 0.5f);
                
                Vector3 rXAxis = (rLine.End - rLine.Start).normalized;
                Vector3 rYAxis = rLine.Up.normalized;
                Vector3 rZAxis = Vector3.Cross(rYAxis, rXAxis).normalized;

                this.OBB = new OBB(rCenter, rExtends, rXAxis, rYAxis, rZAxis);
                this.VerticalPlane = new Plane(rZAxis, rCenter);
            }

            public bool IsContainPoint(Vector3 rPoint)
            {
                if (this.OBB == null) return false;
                return this.OBB.ContainPoint(rPoint);
            }

            public Vector3 GetRealPos(Vector3 rPoint)
            {
                float rVDist = this.VerticalPlane.GetDistanceToPoint(rPoint);
                return rPoint - rVDist * this.OBB.ZAxis;
            }
        }
        
        public List<LineBox>        LineBoxes;

        public LinePathAlgorithm(List<Line> rLines)
        {
            this.LineBoxes = new List<LineBox>();
            for (int i = 0; i < rLines.Count; i++)
            {
                LineBox rLineBox = new LineBox(rLines[i]);
                this.LineBoxes.Add(rLineBox);
            }
        }

        public Vector3 GetRealPos(Vector3 rPoint)
        {
            for (int i = 0; i < this.LineBoxes.Count; i++)
            {
                if (this.LineBoxes[i].IsContainPoint(rPoint))
                {
                    return this.LineBoxes[i].GetRealPos(rPoint);
                }
            }
            return rPoint;
        }
    }
}
