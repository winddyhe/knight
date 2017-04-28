using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Math.AreaOverlap
{
    public class AreaOverlapAlgorithm
    {
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
                        
                        Debug.DrawLine(rLine.Start, rLine.End, rColor);
                    }
                }
            }
        }

        private void BuildAreas()
        {
            var rPrevAreas = new List<AreaAABB>(this.mCurrentAreas);
            this.mCurrentAreas.Clear();

            for (int i = 0; i < rPrevAreas.Count; i++)
            {
                rPrevAreas[i].InitSides();
                this.BuildArea(rPrevAreas[i]);
                mCurrentAreas.Add(rPrevAreas[i]);
            }
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
            List<Line> rTempLines = new List<Line>();
            for (int i = 0; i < rSide.Lines.Count; i++)
            {
                Line rLine = rSide.Lines[i];

                for (int j = 0; j < rArea.Sides.Count; j++)
                {
                    List<Line> rTempAreaLines = new List<Line>();
                    for (int k = 0; k < rArea.Sides[j].Lines.Count; k++)
                    {
                        Line rAreaLine = rArea.Sides[j].Lines[k];
                        this.GetIntersectPoint_AABB(rLine, rAreaLine, rTempLines, rTempAreaLines);
                    }
                    rArea.Sides[j].Lines = rTempAreaLines;
                }
            }
            rSide.Lines = rTempLines;

            // 设置Side中Line的绘制状态
            for (int i = 0; i < rSide.Lines.Count; i++)
            {
                if (rArea.IsContainLine(rSide.Lines[i]))
                {
                    rSide.Lines[i].State = 1;       // 为 1 表示不绘制隐藏
                }
            }
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

        private void AddLineToList_NoSame(List<Line> rLines, Line rNewLine)
        {
            int nIndex = rLines.FindIndex((rItem) => { return Line.Equals(rItem, rNewLine); });
            if (nIndex >= 0) return;
            rLines.Add(rNewLine);
        }

        /// <summary>
        /// 求两条AABB直线的交点
        /// </summary>
        public void GetIntersectPoint_AABB(Line rLine1, Line rLine2, List<Line> rOutLines1, List<Line> rOutLines2)
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
                        if (rLine1.End.z <= rLine2.Start.z)
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                        else if (rLine1.End.z < rLine2.End.z && rLine1.End.z > rLine2.Start.z)
                        {
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine1.Start, End = rLine2.Start });
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine2.Start, End = rLine1.End });

                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine2.Start, End = rLine1.End });
                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine1.End, End = rLine2.End });
                        }
                        else if (rLine1.End.z >= rLine2.End.z)
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                    }
                    else if (rLine1.Start.z >= rLine2.Start.z && rLine1.Start.z <= rLine2.End.z)    // Line1的起点在Line2之中
                    {
                        if (rLine1.End.z <= rLine2.End.z)
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                        else if (rLine1.End.z > rLine2.End.z)
                        {
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine1.Start, End = rLine2.End });
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine2.End, End = rLine1.End });

                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine2.Start, End = rLine1.Start });
                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine1.Start, End = rLine2.End });
                        }
                    }
                    else if (rLine1.Start.z > rLine2.End.z)   // Line1起点在Line2终点大
                    {
                        AddLineToList_NoSame(rOutLines1, rLine1);
                        AddLineToList_NoSame(rOutLines2, rLine2);
                    }
                }
                else
                {
                    AddLineToList_NoSame(rOutLines1, rLine1);
                    AddLineToList_NoSame(rOutLines2, rLine2);
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
                        if (rLine1.End.x <= rLine2.Start.x)     // Line1的终点比Line2的终点小
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                        else if (rLine1.End.x < rLine2.End.x && rLine1.End.x > rLine2.Start.x)      // Line1的终点在Line2之间
                        {
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine1.Start, End = rLine2.Start });
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine2.Start, End = rLine1.End });

                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine2.Start, End = rLine1.End });
                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine1.End, End = rLine2.End });
                        }
                        else if (rLine1.End.x >= rLine2.End.x)
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                    }
                    else if (rLine1.Start.x >= rLine2.Start.x && rLine1.Start.x <= rLine2.End.x)    // Line1的起点在Line2之中
                    {
                        if (rLine1.End.x <= rLine2.End.x)
                        {
                            AddLineToList_NoSame(rOutLines1, rLine1);
                            AddLineToList_NoSame(rOutLines2, rLine2);
                        }
                        else if (rLine1.End.x > rLine2.End.z)
                        {
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine1.Start, End = rLine2.End });
                            AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine2.End, End = rLine1.End });

                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine2.Start, End = rLine1.Start });
                            AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine1.Start, End = rLine2.End });
                        }
                    }
                    else if (rLine1.Start.x > rLine2.End.x)
                    {
                        AddLineToList_NoSame(rOutLines1, rLine1);
                        AddLineToList_NoSame(rOutLines2, rLine2);
                    }
                }
                else
                {
                    AddLineToList_NoSame(rOutLines1, rLine1);
                    AddLineToList_NoSame(rOutLines2, rLine2);
                }
            }
            else // 两条线是垂直的
            {
                if (rLine1.IsIntersect(rLine2)) // 如果两个线段相交
                {
                    var rIntersectPoint = rLine1.CalcIntersectPoint(rLine2);

                    AddLineToList_NoSame(rOutLines1, new Line() { Start = rLine1.Start, End = rIntersectPoint });
                    AddLineToList_NoSame(rOutLines1, new Line() { Start = rIntersectPoint, End = rLine1.End });

                    AddLineToList_NoSame(rOutLines2, new Line() { Start = rLine2.Start, End = rIntersectPoint });
                    AddLineToList_NoSame(rOutLines2, new Line() { Start = rIntersectPoint, End = rLine2.End });
                }
                else    // 如果不相交
                {
                    AddLineToList_NoSame(rOutLines1, rLine1);
                    AddLineToList_NoSame(rOutLines2, rLine2);
                }
            }
        }
    }
}
