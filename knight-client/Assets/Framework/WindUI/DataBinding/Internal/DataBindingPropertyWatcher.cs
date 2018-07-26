using System;
using System.Collections.Generic;
using System.Linq;
using Knight.Core;

namespace UnityEngine.UI
{
    public class DataBindingPropertyWatcher
    {
        private object  mPropertyOwner;
        private string  mPropertyName;

        private Action  mAction;
        
        public DataBindingPropertyWatcher(object rPropOwner, string rPropName, Action rAction)
        {
            this.mPropertyOwner = rPropOwner;
            this.mPropertyName  = rPropName;
            this.mAction        = rAction;
        }

        public void PropertyChanged(string rPropName)
        {
            if (!this.mPropertyName.Equals(rPropName)) return;

            UtilTool.SafeExecute(this.mAction);
        }
    }
}
