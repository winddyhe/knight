using System;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    public partial class DataBindingOneWay
    {        
        [HideInInspector]
        public  string[]        ModelPaths;

        private bool IsShowCurViewPath()
        {
            return this.ModelPaths.Length != 0 && !string.IsNullOrEmpty(this.CurModelPath);
        }
    }
}
