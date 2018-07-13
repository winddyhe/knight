using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public partial class DataBindingOneWay
    {
        [HideInInspector]
        public  string[]                ModelPaths;
        [HideInInspector]
        public  string[]                ViewPaths;

        [HideInInspector]
        public  List<ModelDataItem>     DataModelItems;
        [HideInInspector]
        public  List<ViewDataItem>      DataViewItems;

        private bool IsShowCurViewPath()
        {
            return this.ModelPaths.Length != 0 && !string.IsNullOrEmpty(this.CurModelPath);
        }
        
        public void GetAllModelPaths()
        {
            this.DataModelItems = DataBindingTypeResolve.GetAllModelPaths(this.gameObject);
            this.ModelPaths = new string[this.DataModelItems.Count];
            for (int i = 0; i < this.DataModelItems.Count; i++)
            {
                this.ModelPaths[i] = this.DataModelItems[i].Path;
            }
        }

        public void GetAllViewPaths()
        {
            var rDataModelItem = this.DataModelItems.Find((rItem) => { return rItem.Path.Equals(this.CurModelPath); });
            if (rDataModelItem != null)
            {
                this.DataViewItems = DataBindingTypeResolve.GetAllViewPaths(rDataModelItem, this.gameObject);
                this.ViewPaths = new string[this.DataViewItems.Count];
                for (int i = 0; i < this.DataViewItems.Count; i++)
                {
                    this.ViewPaths[i] = this.DataViewItems[i].Path;
                }
            }
        }

        public void SetCurData()
        {
            this.CurModelData = this.DataModelItems.Find((rItem) => { return rItem.Path.Equals(this.CurModelPath); });
            this.CurViewData  = this.DataViewItems.Find((rItem)  => { return rItem.Path.Equals(this.CurViewPath);   });
        }
    }
}
