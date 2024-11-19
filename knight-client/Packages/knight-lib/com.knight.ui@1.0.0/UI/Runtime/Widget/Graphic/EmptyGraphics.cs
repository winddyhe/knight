using UnityEngine.UI;

namespace Knight.Framework.UI
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
