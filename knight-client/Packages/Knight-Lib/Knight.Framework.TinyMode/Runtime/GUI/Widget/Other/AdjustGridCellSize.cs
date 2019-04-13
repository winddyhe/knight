//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI
{
    [ExecuteInEditMode]
    public class AdjustGridCellSize : MonoBehaviour
    {
        public GridLayoutGroup Grid;

        void Awake()
        {
            this.Grid = this.GetComponent<GridLayoutGroup>();
            var rect = GetComponent<RectTransform>().rect;
            Grid.cellSize = new Vector2(rect.width / 2, Grid.cellSize.y);
        }
        
        void Update()
        {
#if UNITY_EDITOR
            if (this.Grid == null)
                this.Grid = this.GetComponent<GridLayoutGroup>();
            if (this.Grid == null)
                return;

            var rect = GetComponent<RectTransform>().rect;
            Grid.cellSize = new Vector2(rect.width / 3, Grid.cellSize.y);
#endif
        }
    }
}


