//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using Knight.Core;

namespace Knight.Framework.TinyMode.UI
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
