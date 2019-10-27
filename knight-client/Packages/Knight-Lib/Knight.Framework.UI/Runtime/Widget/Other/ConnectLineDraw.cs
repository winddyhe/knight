using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public class ConnectLineDraw : MaskableGraphic
    {
        [SerializeField]
        private Color m_LineColor = new Color(0,0,0,1);
        [SerializeField]
        private float m_Linewidth = 20f;
        [SerializeField]
        private Vector3 mStartPos;
        [SerializeField]
        private Vector3 mEndPos;

        public Vector3 EndPos
        {
            get { return mEndPos; }
            set
            {
                mEndPos = value - this.GetComponent<RectTransform>().transform.localPosition - new Vector3(100, 0, 0);
                this.SetVerticesDirty();
            }
        }

        public Vector3 StartPos
        {
            get { return mStartPos; }
            set
            {
                mStartPos = value - this.GetComponent<RectTransform>().transform.localPosition- new Vector3(100,0,0);
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
                vh.Clear();
            //if(this.StartPos.x == 0 && this.StartPos.y == 0)
            //{
            //    return;
            //}
                UIVertex[] Verts = new UIVertex[4];
                Verts[0].position = this.StartPos;
                Verts[0].color = this.m_LineColor;
                Verts[0].uv0 = new Vector2(0,0);

                Verts[1].position = new Vector2(this.StartPos.x, this.StartPos.y + this.m_Linewidth);
                Verts[1].color = this.m_LineColor;
                Verts[1].uv0 = new Vector2(1, 0);

                Verts[2].position = new Vector2(this.EndPos.x, this.EndPos.y + this.m_Linewidth);
                Verts[2].color = this.m_LineColor;
                Verts[2].uv0 = new Vector2(1, 1);

                Verts[3].position = this.EndPos;
                Verts[3].color = this.m_LineColor;
                Verts[3].uv0 = new Vector2(0, 1);

                vh.AddUIVertexQuad(Verts);
        }
    }
}
