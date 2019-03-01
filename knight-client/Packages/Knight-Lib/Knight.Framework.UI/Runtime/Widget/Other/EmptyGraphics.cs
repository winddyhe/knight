using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public class EmptyGraphics : Graphic
    {
        protected override void UpdateMaterial()
        {
        }
        protected override void UpdateGeometry()
        {
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}
