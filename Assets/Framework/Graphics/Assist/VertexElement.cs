//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Graphics
{
    public class VertexElement
    {
        public List<Vector3>    Vertices;
        public List<Vector2>    UVs;
        public List<Color32>    Colors;
        public List<int>        Indices;

        public VertexElement()
        {
            this.Vertices = VertexListPool<Vector3>.Alloc();
            this.UVs = VertexListPool<Vector2>.Alloc();
            this.Colors = VertexListPool<Color32>.Alloc();
            this.Indices = VertexListPool<int>.Alloc();
        }

        public void Clear()
        {
            this.Vertices.Clear();
            this.UVs.Clear();
            this.Colors.Clear();
            this.Indices.Clear();
        }

        public void AddVert(Vector3 rPosition, Color32 rColor, Vector2 rUV0)
        {
            this.Vertices.Add(rPosition);
            this.Colors.Add(rColor);
            this.UVs.Add(rUV0);
        }

        public void Free()
        {
            VertexListPool<Vector3>.Free(this.Vertices);
            VertexListPool<Color32>.Free(this.Colors);
            VertexListPool<Vector2>.Free(this.UVs);
            VertexListPool<int>.Free(this.Indices);

            this.Vertices = null;
            this.Colors = null;
            this.UVs = null;
            this.Indices = null;
        }

        public void AddTriangle(int nIndex0, int nIndex1, int nIndex2)
        {
           this.Indices.Add(nIndex0);
           this.Indices.Add(nIndex1);
           this.Indices.Add(nIndex2);
        }

        public void SetMeshVertices(Mesh rMesh)
        {
            rMesh.SetVertices(this.Vertices);
            rMesh.SetUVs(0, this.UVs);
            rMesh.SetColors(this.Colors);
        }

        public void SetMeshIndices(Mesh rMesh)
        {
            rMesh.SetIndices(this.Indices.ToArray(), MeshTopology.Triangles, 0);
        }
    }
}
