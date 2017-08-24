using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Math;

namespace Test
{
    [ExecuteInEditMode]
    public class PathTest : MonoBehaviour
    {
        public List<GameObject>             GameObjects;
        public List<LinePathAlgorithm.Line> Lines;

        public float                        Width;
        public float                        Height;

        public LinePathAlgorithm            LinePathAlgo;

        void Start()
        {

        }

        void OnDrawGizmos()
        {
            if (this.GameObjects == null || this.GameObjects.Count < 2) return;

            for (int i = 0; i < this.GameObjects.Count-1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(this.GameObjects[i].transform.position, this.GameObjects[i + 1].transform.position);
            }

            //if (this.Lines != null)
            //{
            //    for (int i = 0; i < this.Lines.Count; i++)
            //    {
            //        this.DrawLine(this.Lines[i]);
            //    }
            //}

            if (this.LinePathAlgo != null)
            {
                for (int i = 0; i < this.LinePathAlgo.LineBoxes.Count; i++)
                {
                    this.LinePathAlgo.LineBoxes[i].DrawGizmos();
                }
            }
        }

        private void DrawLine(LinePathAlgorithm.Line rLine)
        {
            Gizmos.DrawLine(rLine.Start, rLine.End);
            Gizmos.DrawLine(rLine.Start, rLine.Start + rLine.Up * rLine.Height);
            Gizmos.DrawLine(rLine.End, rLine.End + rLine.Up * rLine.Height);
            Gizmos.DrawLine(rLine.Start + rLine.Up * rLine.Height, rLine.End + rLine.Up * rLine.Height);
        }

        [ContextMenu("获取所有的Lines")]
        public void GetAllLines()
        {
            this.Lines = new List<LinePathAlgorithm.Line>();
            for (int i = 0; i < this.GameObjects.Count-1; i++)
            {
                LinePathAlgorithm.Line rLine = new LinePathAlgorithm.Line();
                rLine.Start = this.GameObjects[i].transform.position;
                rLine.End = this.GameObjects[i+1].transform.position;
                rLine.Up = Vector3.up;
                rLine.Width = this.Width;
                rLine.Height = this.Height;
                this.Lines.Add(rLine);
            }
            this.LinePathAlgo = new LinePathAlgorithm(this.Lines);
        }
    }
}
