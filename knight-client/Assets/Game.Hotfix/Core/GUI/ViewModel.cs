using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using Knight.Core;

namespace Knight.Hotfix.Core
{
    public class ViewModel
    {
        public    Action<string>    PropChangedHandler;

        public void PropChanged(string rPropName)
        {
            UtilTool.SafeExecute(this.PropChangedHandler, rPropName);
        }
    }
}
