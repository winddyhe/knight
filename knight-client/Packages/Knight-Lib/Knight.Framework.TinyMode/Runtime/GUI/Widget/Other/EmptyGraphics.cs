//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI
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
