using FancyScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class FancyScrollRectCellItem : FancyScrollRectCell<FancyScrollCellData, FancyScrollRectCellContext>
    {
        public ViewModelDataSourceListTemplate DataSource;
        public CanvasGroup CanvasGroup;

        private FancyScrollCellBridge mFancyCellBridge;

        public override void Initialize()
        {
            this.mFancyCellBridge = this.GetComponent<FancyScrollCellBridge>();
            this.mFancyCellBridge.Initialize();
        }

        public override void UpdateContent(FancyScrollCellData rItemData)
        {
            this.mFancyCellBridge.UpdateContent(this.Index, rItemData);
        }

        protected override void UpdatePosition(float rNormalizedPosition, float fLocalPosition)
        {
            base.UpdatePosition(rNormalizedPosition, fLocalPosition);
        }

        public override void SetVisible(bool visible)
        {
            this.CanvasGroup.alpha = visible ? 1 : 0;
            this.CanvasGroup.interactable = visible;
            this.transform.localScale = visible ? Vector3.one : Vector3.zero;
        }

        public override bool IsVisible => this.CanvasGroup.alpha > 0;
    }
}
