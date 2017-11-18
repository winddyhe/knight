//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Math
{
    public class AreaOverlapAlgorithm
    {
        public class Line
        {
            public int      State;
            public Vector3  Start;
            public Vector3  End;

            public Color    Color = new Color(UnityEngine.Random.Range(0, 255) / 255.0f, UnityEngine.Random.Range(0, 255) / 255.0f, UnityEngine.Random.Range(0, 255) / 255.0f);

            public static bool Equals(Line rLine1, Line rLine2)
            {
                return (rLine1.Start == rLine2.Start && rLine1.End == rLine2.End) ||
                        (rLine1.Start == rLine2.End && rLine1.End == rLine2.Start);
            }

            public void Sort(int nAxis)
            {
                if (nAxis == 0)             // 按照X轴排序
                {
                    if (this.Start.x > this.End.x)
                    {
                        Vector3 rTemp = this.End;
                        this.End = Start;
                        this.Start = rTemp;
                    }
                }
                else if (nAxis == 1)        // 按照Z轴排序
                {
                    if (this.Start.z > this.End.z)
                    {
                        Vector3 rTemp = this.End;
                        this.End = Start;
                        this.Start = rTemp;
                    }
                }
            }

            public void FixLine(int nSideIndex)
            {
                if (nSideIndex == 0 && this.Start.x >= this.End.x)
                {
                    var temp = this.Start;
                    this.Start = this.End;
                    this.End = temp;
                }
                else if (nSideIndex == 1 && this.Start.z >= this.End.z)
                {
                    var temp = this.Start;
                    this.Start = this.End;
                    this.End = temp;
                }
                else if (nSideIndex == 2 && this.Start.x < this.End.x)
                {
                    var temp = this.Start;
                    this.Start = this.End;
                    this.End = temp;
                }
                else if (nSideIndex == 3 && this.Start.z < this.End.z)
                {
                    var temp = this.Start;
                    this.Start = this.End;
                    this.End = temp;
                }
            }

            public bool IsPoint()
            {
                return this.Start.x == this.End.x && this.Start.z == this.End.z;
            }

            #region 判断线段是否相交，并求出交点
            private float determinant(float v1, float v2, float v3, float v4)  // 行列式  
            {
                return (v1 * v4 - v2 * v3);
            }

            private bool intersect3(Vector3 aa, Vector3 bb, Vector3 cc, Vector3 dd)
            {
                double delta = determinant(bb.x - aa.x, cc.x - dd.x, bb.z - aa.z, cc.z - dd.z);
                if (delta <= (1e-6) && delta >= -(1e-6))  // delta=0，表示两线段重合或平行  
                {
                    return false;
                }
                double namenda = determinant(cc.x - aa.x, cc.x - dd.x, cc.z - aa.z, cc.z - dd.z) / delta;
                if (namenda > 1 || namenda < 0)
                {
                    return false;
                }
                double miu = determinant(bb.x - aa.x, cc.x - aa.x, bb.z - aa.z, cc.z - aa.z) / delta;
                if (miu > 1 || miu < 0)
                {
                    return false;
                }
                return true;
            }

            public bool IsIntersect(Line rLine)
            {
                return this.intersect3(this.Start, this.End, rLine.Start, rLine.End);
            }

            private float Cross(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
            {
                return (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);
            }

            private float Area(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                return Cross(p1, p2, p1, p3);
            }

            float fArea(Vector3 p1, Vector3 p2, Vector3 p3)
            {
                return Mathf.Abs(Area(p1, p2, p3));
            }

            private Vector3 Inter(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
            {
                float k = fArea(p1, p2, p3) / fArea(p1, p2, p4);
                return new Vector3((p3.x + k * p4.x) / (1 + k), 0, (p3.z + k * p4.z) / (1 + k));
            }

            public Vector3 CalcIntersectPoint(Line rLine)
            {
                return this.Inter(this.Start, this.End, rLine.Start, rLine.End);
            }
            #endregion
        }

        public class AreaSide
        {
            public int              Index;
            public List<Vector3>    Points;
            public Line             EdgeLine;
            public List<Line>       Lines;

            public AreaSide()
            {
                this.Index = 0;
                this.Points = new List<Vector3>();
            }

            public void SortPoints()
            {
                List<Vector3> rTempPoints = new List<Vector3>();
                for (int i = 0; i < this.Points.Count; i++)
                {
                    int nFindIndex = 0;
                    if (Index == 0)
                    {
                        nFindIndex = rTempPoints.FindIndex((rItem) => { return rItem.x <= this.Points[i].x; });
                    }
                    else if (Index == 1)
                    {
                        nFindIndex = rTempPoints.FindIndex((rItem) => { return rItem.z <= this.Points[i].z; });
                    }
                    else if (Index == 2)
                    {
                        nFindIndex = rTempPoints.FindIndex((rItem) => { return rItem.x >= this.Points[i].x; });
                    }
                    else if (Index == 3)
                    {
                        nFindIndex = rTempPoints.FindIndex((rItem) => { return rItem.z >= this.Points[i].z; });
                    }
                    rTempPoints.Insert(nFindIndex + 1, this.Points[i]);
                }
                this.EdgeLine.FixLine(this.Index);
                this.Points = rTempPoints;

                this.Lines = new List<Line>();
                Vector3 rStart = this.EdgeLine.Start;
                Vector3 rEnd = this.EdgeLine.Start;
                for (int k = 0; k < this.Points.Count; k++)
                {
                    rEnd = this.Points[k];
                    this.Lines.Add(new Line() { Start = rStart, End = rEnd });
                    rStart = this.Points[k];
                }
                rEnd = this.EdgeLine.End;
                this.Lines.Add(new Line() { Start = rStart, End = rEnd });

            }
        }

        public class AABB
        {
            public Vector3 Max;
            public Vector3 Min;

            public bool IsContainPoint(Vector3 rPos)
            {
                bool bResult = rPos.x - this.Min.x > -0.001f && rPos.x - this.Max.x < 0.001f &&
                               rPos.z - this.Min.z > -0.001f && rPos.z - this.Max.z < 0.001f;
                return bResult;
            }

            public bool IsContainLine(Line rLine1)
            {
                return IsContainPoint(rLine1.Start) && IsContainPoint(rLine1.End);
            }

            public bool IsContainLine_NoEdge(Line rLine1)
            {
                bool bIsInEdge = (rLine1.Start.x == rLine1.End.x && (rLine1.Start.x == this.Min.x || rLine1.Start.x == this.Max.x)) ||
                                 (rLine1.Start.z == rLine1.End.z && (rLine1.Start.z == this.Min.z || rLine1.Start.z == this.Max.z));
                return (IsContainPoint(rLine1.Start) && IsContainPoint(rLine1.End)) && !bIsInEdge;
            }
        }

        public class AreaAABB
        {
            public string           AreaID;

            public AABB             Rect;
            public List<AreaSide>   Sides;

            public int              Order;
            public int              UID;

            public AreaAABB(string rAreaID, int nUID, int nOrder, Vector3 rPos, Vector3 rSize)
            {
                this.AreaID = rAreaID;
                this.UID = nUID;
                this.Order = nOrder;

                this.Rect = new AABB();
                this.Rect.Min = rPos - rSize * 0.5f;
                this.Rect.Max = rPos + rSize * 0.5f;

                this.InitSides();
            }

            public void InitSides()
            {
                this.Sides = new List<AreaSide>();

                AreaSide rDownSide = new AreaSide() { Index = 0, Points = new List<Vector3>() };
                rDownSide.EdgeLine = new Line() { Start = this.Rect.Min, End = new Vector3(this.Rect.Max.x, 0, this.Rect.Min.z) };  // min.z 一样
                rDownSide.SortPoints();
                this.Sides.Add(rDownSide);

                AreaSide rRightSide = new AreaSide() { Index = 1, Points = new List<Vector3>() };
                rRightSide.EdgeLine = new Line() { Start = new Vector3(this.Rect.Max.x, 0, this.Rect.Min.z), End = this.Rect.Max }; // Max.x 一样
                rRightSide.SortPoints();
                this.Sides.Add(rRightSide);

                AreaSide rUpSide = new AreaSide() { Index = 2, Points = new List<Vector3>() };
                rUpSide.EdgeLine = new Line() { Start = this.Rect.Max, End = new Vector3(this.Rect.Min.x, 0, this.Rect.Max.z) };    // Max.z 一样
                rUpSide.SortPoints();
                this.Sides.Add(rUpSide);

                AreaSide rLeftSide = new AreaSide() { Index = 3, Points = new List<Vector3>() };
                rLeftSide.EdgeLine = new Line() { Start = new Vector3(this.Rect.Min.x, 0, this.Rect.Max.z), End = this.Rect.Min }; // Min.x 一样
                rLeftSide.SortPoints();
                this.Sides.Add(rLeftSide);
            }

            public bool IsContainPoint(Vector3 rPos)
            {
                return this.Rect.IsContainPoint(rPos);
            }

            public bool IsContainLine(Line rLine1)
            {
                return this.Rect.IsContainLine(rLine1);
            }

            public bool IsContainLine_NoEdge(Line rLine1)
            {
                return this.Rect.IsContainLine_NoEdge(rLine1);
            }
        }

        public List<AreaAABB>   mCurrentAreas;

        public AreaOverlapAlgorithm()
        {
            this.mCurrentAreas = new List<AreaAABB>();
        }

        public void AddArea(string rAreaID, int nUID, int nOrder, Vector3 rPos, Vector3 rSize)
        {
            int nFindIndex = mCurrentAreas.FindIndex((rItem) => { return rItem.AreaID.Equals(rAreaID); });
            if (nFindIndex >= 0) return;

            var rNewArea = new AreaAABB(rAreaID, nUID, nOrder, rPos, rSize);
            mCurrentAreas.Add(rNewArea);
            mCurrentAreas.Sort((rItem1, rItem2) => { return rItem1.Order.CompareTo(rItem2.Order); });
            this.BuildAreas();
        }

        public void RemoveArea(string rAreaID)
        {
            int nFindIndex = mCurrentAreas.FindIndex((rItem) => { return rItem.AreaID.Equals(rAreaID); });
            if (nFindIndex < 0) return;

            mCurrentAreas.RemoveAt(nFindIndex);
            mCurrentAreas.Sort((rItem1, rItem2) => { return rItem1.Order.CompareTo(rItem2.Order); });
            this.BuildAreas();
        }

        public void DebugDraw()
        {
            if (mCurrentAreas == null) return;

            for (int i = 0; i < mCurrentAreas.Count; i++)
            {
                Color rColor = Color.yellow;
                if (mCurrentAreas[i].AreaID == "Area1")
                    rColor = Color.red;
                else if (mCurrentAreas[i].AreaID == "Area2")
                    rColor = Color.green;
                else if (mCurrentAreas[i].AreaID == "Area3")
                    rColor = Color.blue;
                
                for (int j = 0; j < mCurrentAreas[i].Sides.Count; j++)
                {
                    for (int k = 0; k < mCurrentAreas[i].Sides[j].Lines.Count; k++)
                    {
                        Line rLine = mCurrentAreas[i].Sides[j].Lines[k];
                        if (rLine.State == 1) continue;
                        Debug.DrawLine(rLine.Start, rLine.End, rLine.Color);
                    }
                }
            }
        }

        private void BuildAreas()
        {
            var rPrevAreas = new List<AreaAABB>(this.mCurrentAreas);
            this.mCurrentAreas.Clear();

            // 找到所有的区域与区域之间线段的交点
            for (int i = 0; i < rPrevAreas.Count; i++)
            {
                rPrevAreas[i].InitSides();
                this.BuildArea(rPrevAreas[i]);
                mCurrentAreas.Add(rPrevAreas[i]);
            }

            // 根据线段的交点找到哪些线是需要隐藏的
            for (int i = 1; i < mCurrentAreas.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    for (int k = 0; k < mCurrentAreas[i].Sides.Count; k++)
                    {
                        AreaSide rSide = mCurrentAreas[i].Sides[k];
                        for (int p = 0; p < rSide.Lines.Count; p++)
                        {
                            if (mCurrentAreas[j].IsContainLine(rSide.Lines[p]))
                            {
                                rSide.Lines[p].State = 1;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < mCurrentAreas.Count; i++)
            {
                // 找Area中的线所在的所有所属区域
                for (int j = 0; j < mCurrentAreas[i].Sides.Count; j++)
                {
                    for (int k = 0; k < mCurrentAreas[i].Sides[j].Lines.Count; k++)
                    {
                        Line rLine = mCurrentAreas[i].Sides[j].Lines[k];
                        AreaAABB rHighestOrderArea = GetHighestOrderArea(rLine, mCurrentAreas[i]);
                        if (rHighestOrderArea != null)
                        {
                            if (rHighestOrderArea.UID == mCurrentAreas[i].UID)
                                rLine.State = 1;
                        }
                    }
                }
            }
        }

        private AreaAABB GetHighestOrderArea(Line rLine, AreaAABB rSelfArea)
        {
            AreaAABB rArea = null;
            for (int i = 0; i < mCurrentAreas.Count; i++)
            {
                if (mCurrentAreas[i] == rSelfArea) continue;
                if (mCurrentAreas[i].IsContainLine_NoEdge(rLine))
                {
                    if (rArea == null || mCurrentAreas[i].Order < rArea.Order)
                        rArea = mCurrentAreas[i];
                }
            }
            return rArea;
        }

        private void BuildArea(AreaAABB rNewAABB)
        {
            for (int i = 0; i < rNewAABB.Sides.Count; i++)
            {
                for (int k = 0; k < mCurrentAreas.Count; k++)
                {
                    this.Intersect_Side_AreaAABB(rNewAABB.Sides[i], mCurrentAreas[k]);
                }
            }
        }

        /// <summary>
        /// 判断一个Side是否和一个Area相交，如果相交了，分别把Side和AABB的边裁减成多个线段
        /// </summary>
        public void Intersect_Side_AreaAABB(AreaSide rSide, AreaAABB rArea)
        {
            List<Vector3> rTempPoints = new List<Vector3>(rSide.Points);
            Line rLine = rSide.EdgeLine;
            for (int j = 0; j < rArea.Sides.Count; j++)
            {
                List<Vector3> rTempAreaPoints = new List<Vector3>(rArea.Sides[j].Points);
                Line rAreaLine = rArea.Sides[j].EdgeLine;
                this.GetIntersectPoint_AABB(rLine, rAreaLine, rTempPoints, rTempAreaPoints);
                rArea.Sides[j].Points = rTempAreaPoints;
                rArea.Sides[j].SortPoints();
            }
            rSide.Points = rTempPoints;
            rSide.SortPoints();
        }

        /// <summary>
        /// 判断两条线是否平行
        /// return: 
        ///     0 - 不平行
        ///     1 - 平行于Z轴
        ///     2 - 平行于X轴
        /// </summary>
        public int Parallel_AABB(Line rLine1, Line rLine2)
        {
            if (rLine1.Start.x == rLine1.End.x && rLine2.Start.x == rLine2.End.x) return 1;

            if (rLine1.Start.z == rLine1.End.z && rLine2.Start.z == rLine2.End.z) return 2;

            return 0;
        }

        private void AddPointToList_NoSame(List<Vector3> rLines, Vector3 rNewLine)
        {
            int nIndex = rLines.FindIndex((rItem) => { return rItem == rNewLine; });
            if (nIndex >= 0) return;
            rLines.Add(rNewLine);
        }

        /// <summary>
        /// 求两条AABB直线的交点
        /// </summary>
        public void GetIntersectPoint_AABB(Line rLine1, Line rLine2, List<Vector3> rOutPoints1, List<Vector3> rOutPoints2)
        {
            int nParallel = this.Parallel_AABB(rLine1, rLine2);
            if (nParallel == 1)     // 如果两条线平行于Z轴
            {
                if (rLine1.Start.x == rLine2.Start.x && rLine1.End.x == rLine2.End.x) // 如果两条线段重合了
                {
                    // 两条直线按照Z轴排序
                    rLine1.Sort(1);
                    rLine2.Sort(1);

                    if (rLine1.Start.z < rLine2.Start.z)        // Line1的起点比Line2的起点小
                    {
                        if (rLine1.End.z < rLine2.End.z && rLine1.End.z > rLine2.Start.z)
                        {
                            AddPointToList_NoSame(rOutPoints1, rLine2.Start);
                            AddPointToList_NoSame(rOutPoints2, rLine1.End);
                        }
                    }
                    else if (rLine1.Start.z >= rLine2.Start.z && rLine1.Start.z <= rLine2.End.z)    // Line1的起点在Line2之中
                    {
                        if (rLine1.End.z > rLine2.End.z)
                        {
                            AddPointToList_NoSame(rOutPoints1, rLine2.End);
                            AddPointToList_NoSame(rOutPoints2, rLine1.Start);
                        }
                    }
                }
            }
            else if (nParallel == 2) // 如果两条线平行于X轴
            {
                if (rLine1.Start.z == rLine2.Start.z && rLine1.End.z == rLine2.End.z) // 如果两条线段重合了
                {
                    // 两条直线按照X轴排序
                    rLine1.Sort(0);
                    rLine2.Sort(0);

                    if (rLine1.Start.x < rLine2.Start.x)        // Line1的起点比Line2的起点小
                    {
                        if (rLine1.End.x < rLine2.End.x && rLine1.End.x > rLine2.Start.x)           // Line1的终点在Line2之间
                        {
                            AddPointToList_NoSame(rOutPoints1, rLine2.Start);
                            AddPointToList_NoSame(rOutPoints2, rLine1.End);
                        }
                    }
                    else if (rLine1.Start.x >= rLine2.Start.x && rLine1.Start.x <= rLine2.End.x)    // Line1的起点在Line2之中
                    {
                        if (rLine1.End.x > rLine2.End.z)
                        {
                            AddPointToList_NoSame(rOutPoints1, rLine2.End);
                            AddPointToList_NoSame(rOutPoints2, rLine1.Start);
                        }
                    }
                }
            }
            else // 两条线是垂直的
            {
                if (rLine1.IsIntersect(rLine2)) // 如果两个线段相交
                {
                    var rIntersectPoint = rLine1.CalcIntersectPoint(rLine2);
                    if (!float.IsNaN(rIntersectPoint.x) && !float.IsNaN(rIntersectPoint.z))
                    {
                        AddPointToList_NoSame(rOutPoints1, rIntersectPoint);
                        AddPointToList_NoSame(rOutPoints2, rIntersectPoint);
                    }
                }
            }
        }
    }
}
