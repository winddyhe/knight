using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Math.AreaOverlap
{
    public class Line
    {
        public int     State;
        public Vector3 Start;
        public Vector3 End;

        public static bool Equals(Line rLine1, Line rLine2)
        {
            return  (rLine1.Start.x == rLine2.Start.x && rLine1.Start.z == rLine2.Start.z && rLine1.End.x == rLine2.End.x   && rLine1.End.z == rLine2.End.z) ||
                    (rLine1.Start.x == rLine2.End.x   && rLine1.Start.z == rLine2.End.z   && rLine1.End.x == rLine2.Start.x && rLine1.End.z == rLine2.Start.z);
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

        private double Determinant(float v1, float v2, float v3, float v4)
        {
            return (v1 * v3 - v2 * v4);
        }

        public bool IsIntersect(Line rLine)
        {
            double delta = Determinant(this.End.x - this.Start.x, rLine.Start.x - rLine.End.x, this.End.y - this.Start.y, rLine.Start.y - rLine.End.y);
            if (delta <= (1e-6) && delta >= -(1e-6))  // delta=0，表示两线段重合或平行  
            {
                return false;
            }
            double namenda = Determinant(rLine.Start.x - this.Start.x, rLine.Start.x - rLine.End.x, rLine.Start.y - this.Start.y, rLine.Start.y - rLine.End.y) / delta;
            if (namenda > 1 || namenda < 0)
            {
                return false;
            }
            double miu = Determinant(this.End.x - this.Start.x, rLine.Start.x - this.Start.x, this.End.y - this.Start.y, rLine.Start.y - this.Start.y) / delta;
            if (miu > 1 || miu < 0)
            {
                return false;
            }
            return true;
        }

        private float Cross(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            return (p2.x - p1.x) * (p4.z - p3.z) - (p2.z - p1.z) * (p4.x - p3.x);
        }

        private float Area(Vector3 p1,Vector3 p2,Vector3 p3)  
        {  
            return Cross(p1,p2,p1,p3);  
        }

        float fArea(Vector3 p1, Vector3 p2, Vector3 p3)  
        {
            return Mathf.Abs(Area(p1, p2, p3));
        }

        public Vector3 Inter(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)  
        {
            float k = fArea(p1, p2, p3) / fArea(p1, p2, p4);
            return new Vector3((p3.x + k * p4.x) / (1 + k), (p3.y + k * p4.y) / (1 + k));
        }

        public Vector3 CalcIntersectPoint(Line rLine)
        {
            return this.Inter(this.Start, this.End, rLine.Start, rLine.End);
        }
    }

    public class AreaSide
    {
        public int              Index;
        public List<Line>       Lines;

        public AreaSide()
        {
            this.Index = 0;
            this.Lines = new List<Line>();
        }

        public void FixSideLines()
        {
            for (int i = 0; i < this.Lines.Count; i++)
            {
                this.Lines[i].FixLine(this.Index);
            }
        }
    }

    public class AABB
    {
        public Vector3          Max;
        public Vector3          Min;

        public bool IsContainPoint(Vector3 rPos)
        {
            return rPos.x >= this.Min.x && rPos.z <= this.Max.x &&
                    rPos.z >= this.Min.z && rPos.z <= this.Max.z;
        }

        public bool IsContainLine(Line rLine1)
        {
            return IsContainPoint(rLine1.Start) && IsContainPoint(rLine1.End);
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

            AreaSide rDownSide = new AreaSide()  { Index = 0, Lines = new List<Line>() };
            rDownSide.Lines.Add(new Line()  { Start = this.Rect.Min, End = new Vector3(this.Rect.Max.x, 0, this.Rect.Min.z) }); // min.z 一样
            this.Sides.Add(rDownSide);

            AreaSide rRightSide = new AreaSide() { Index = 1, Lines = new List<Line>() };
            rRightSide.Lines.Add(new Line() { Start = new Vector3(this.Rect.Max.x, 0, this.Rect.Min.z), End = this.Rect.Max }); // Max.x 一样
            this.Sides.Add(rRightSide);

            AreaSide rUpSide = new AreaSide()    { Index = 2, Lines = new List<Line>() };
            rUpSide.Lines.Add(new Line()    { Start = this.Rect.Max, End = new Vector3(this.Rect.Min.x, 0, this.Rect.Max.z) }); // Max.z 一样
            this.Sides.Add(rUpSide);

            AreaSide rLeftSide = new AreaSide()  { Index = 3, Lines = new List<Line>() };
            rLeftSide.Lines.Add(new Line()  { Start = new Vector3(this.Rect.Min.x, 0, this.Rect.Max.z), End = this.Rect.Min }); // Min.x 一样
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
    }
}