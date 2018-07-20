using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class ViewModel
    {
        public    Action<string>    PropertyChanged;
        public    bool              IsOpened;
        public    bool              IsClosed;

        public object GetPropValue(string rVaribleName)
        {
            Type rType = this.GetType();
            var rModelProp = rType.GetProperty(rVaribleName, HotfixReflectAssists.flags_public);
            object rModelValue = null;
            if (rModelProp != null)
            {
                rModelValue = rModelProp.GetValue(this);
            }
            return rModelValue;
        }
    }
}
