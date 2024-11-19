using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;
using UnityEditor;

namespace Game.Editor
{
    public class GraphicMeshGenerator
    {
        [MenuItem("LogicTools/GraphicMesh/Generate Circle Quad")]
        public static void GenerateQuad_Circle()
        {
            var rGenerateMeshPath = "Assets/Game/Resources/Battle/Debug/Mesh/Circle_QuadMesh.mesh";
            var rQuadMesh = (Mesh)AssetDatabase.LoadAssetAtPath(rGenerateMeshPath, typeof(Mesh));
            if (rQuadMesh == null)
            {
                rQuadMesh = new Mesh();
                AssetDatabase.CreateAsset(rQuadMesh, rGenerateMeshPath);
                AssetDatabase.SaveAssets();
                rQuadMesh = (Mesh)AssetDatabase.LoadAssetAtPath(rGenerateMeshPath, typeof(Mesh));
            }
            rQuadMesh.MarkDynamic();

            var rVertices = new List<Vector3>();
            rVertices.Add(new Vector3(-0.5f, 0.1f, -0.5f));
            rVertices.Add(new Vector3(-0.5f, 0.1f,  0.5f));
            rVertices.Add(new Vector3( 0.5f, 0.1f,  0.5f));
            rVertices.Add(new Vector3( 0.5f, 0.1f, -0.5f));

            var rUVs = new List<Vector2>();
            rUVs.Add(new Vector2(0, 0));
            rUVs.Add(new Vector2(0, 1));
            rUVs.Add(new Vector2(1, 1));
            rUVs.Add(new Vector2(1, 0));

            var rIndices = new List<int>() { 0, 1, 2, 0, 2, 3 };

            rQuadMesh.SetVertices(rVertices);
            rQuadMesh.SetUVs(0, rUVs);
            rQuadMesh.SetIndices(rIndices, MeshTopology.Triangles, 0);

            AssetDatabase.SaveAssetIfDirty(rQuadMesh);
            AssetDatabase.Refresh();
        }

        [MenuItem("LogicTools/GraphicMesh/Generate Rect Quad")]
        public static void GenerateQuad_Rect()
        {
            var rGenerateMeshPath = "Assets/Game/Resources/Battle/Debug/Mesh/Rect_QuadMesh.mesh";
            var rQuadMesh = (Mesh)AssetDatabase.LoadAssetAtPath(rGenerateMeshPath, typeof(Mesh));
            if (rQuadMesh == null)
            {
                rQuadMesh = new Mesh();
                AssetDatabase.CreateAsset(rQuadMesh, rGenerateMeshPath);
                AssetDatabase.SaveAssets();
                rQuadMesh = (Mesh)AssetDatabase.LoadAssetAtPath(rGenerateMeshPath, typeof(Mesh));
            }
            rQuadMesh.MarkDynamic();

            var rVertices = new List<Vector3>();
            rVertices.Add(new Vector3(-0.5f, 0.1f, 0.0f));
            rVertices.Add(new Vector3(-0.5f, 0.1f, 1.0f));
            rVertices.Add(new Vector3( 0.5f, 0.1f, 1.0f));
            rVertices.Add(new Vector3( 0.5f, 0.1f, 0.0f));

            var rUVs = new List<Vector2>();
            rUVs.Add(new Vector2(0, 0));
            rUVs.Add(new Vector2(0, 1));
            rUVs.Add(new Vector2(1, 1));
            rUVs.Add(new Vector2(1, 0));

            var rIndices = new List<int>() { 0, 1, 2, 0, 2, 3 };

            rQuadMesh.SetVertices(rVertices);
            rQuadMesh.SetUVs(0, rUVs);
            rQuadMesh.SetIndices(rIndices, MeshTopology.Triangles, 0);

            AssetDatabase.SaveAssetIfDirty(rQuadMesh);
            AssetDatabase.Refresh();
        }
    }
}

