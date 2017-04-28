using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Math.AreaOverlap;

namespace  Test
{
    public class AreaOverlapTest : MonoBehaviour
    {
        public AreaOverlapAlgorithm AreaAlgo;

        void Awake()
        {
            this.AreaAlgo = new AreaOverlapAlgorithm();
        }

        void OnGUI()
        {
            if (this.AreaAlgo == null) return;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Area 1"))
                this.AreaAlgo.AddArea("Area1", 1, 1, new Vector3(4, 0, -5), new Vector3(50, 0, 50));
            if (GUILayout.Button("Remove Area 1"))
                this.AreaAlgo.RemoveArea("Area1");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Area 2"))
                this.AreaAlgo.AddArea("Area2", 1, 2, new Vector3(-3, 0, 8), new Vector3(50, 0, 50));
            if (GUILayout.Button("Remove Area 2"))
                this.AreaAlgo.RemoveArea("Area2");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Area 3"))
                this.AreaAlgo.AddArea("Area3", 1, 3, new Vector3(4, 0, 5), new Vector3(50, 0, 50));
            if (GUILayout.Button("Remove Area 3"))
                this.AreaAlgo.RemoveArea("Area3");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Area 4"))
                this.AreaAlgo.AddArea("Area4", 1, 4, new Vector3(-30, 0, -30), new Vector3(50, 0, 50));
            if (GUILayout.Button("Remove Area 4"))
                this.AreaAlgo.RemoveArea("Area4");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Remove Area"))
            {
                this.AreaAlgo.RemoveArea("Area1");
                this.AreaAlgo.RemoveArea("Area2");
                this.AreaAlgo.RemoveArea("Area3");
                this.AreaAlgo.RemoveArea("Area4");
            }
        }

        void OnDrawGizmos()
        {
            if (this.AreaAlgo == null) return;
            
            this.AreaAlgo.DebugDraw();
        }
    }
}

