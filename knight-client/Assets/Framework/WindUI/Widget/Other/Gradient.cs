using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField] public Color32 TopColor       = Color.white;
        [SerializeField] public Color32 BottomColor    = Color.black;

        public override void ModifyMesh(VertexHelper rVertexHelper)
        {
            if (!IsActive())
            {
                return;
            }

            var rVertexList = new List<UIVertex>();
            rVertexHelper.GetUIVertexStream(rVertexList);
            int nCount = rVertexList.Count;

            ApplyGradient(rVertexList, 0, nCount);
            rVertexHelper.Clear();
            rVertexHelper.AddUIVertexTriangleStream(rVertexList);
        }

        private void ApplyGradient(List<UIVertex> rVertexList, int nStart, int nEnd)
        {
            float fBottomY = rVertexList[0].position.y;
            float fTopY = rVertexList[0].position.y;
            for (int i = nStart; i < nEnd; ++i)
            {
                float y = rVertexList[i].position.y;
                if (y > fTopY)
                {
                    fTopY = y;
                }
                else if (y < fBottomY)
                {
                    fBottomY = y;
                }
            }

            float fUIElementHeight = fTopY - fBottomY;
            for (int i = nStart; i < nEnd; ++i)
            {
                UIVertex rUIVertex = rVertexList[i];
                rUIVertex.color = Color32.Lerp(this.BottomColor, this.TopColor, (rUIVertex.position.y - fBottomY) / fUIElementHeight);
                rVertexList[i] = rUIVertex;
            }
        }
    }
}