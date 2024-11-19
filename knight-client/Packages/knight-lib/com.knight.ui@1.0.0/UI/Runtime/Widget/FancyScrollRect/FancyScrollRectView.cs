using FancyScrollView;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class FancyScrollRectView : FancyScrollRect<FancyScrollCellData, FancyScrollRectCellContext>
    {
        public float CellItemSize = 100f;
        public GameObject CellItemPrefab;
        public List<FancyCell<FancyScrollCellData, FancyScrollRectCellContext>> PreCreateCellItems;

        protected override float CellSize => this.CellItemSize;
        protected override GameObject CellPrefab => this.CellItemPrefab;
        protected override List<FancyCell<FancyScrollCellData, FancyScrollRectCellContext>> PreCreateCells => this.PreCreateCellItems;
        protected override bool Scrollable => true;

        public void UpdateData(List<FancyScrollCellData> rItemDatas)
        {
            this.UpdateContents(rItemDatas);
        }
    }
}
