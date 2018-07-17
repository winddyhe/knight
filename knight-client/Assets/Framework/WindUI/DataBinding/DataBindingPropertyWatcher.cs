using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public class DataBindingPropertyWatcher
    {
        private object  mPropertyOwner;
        private string  mPropertyName;

        private Action  mAction;
        private bool    mIsDispose;

        public DataBindingPropertyWatcher(object rPropOwner, string rPropName, Action rAction)
        {
            this.mPropertyOwner = rPropOwner;
            this.mPropertyName  = rPropName;
            this.mAction        = rAction;
        }
    }
}
