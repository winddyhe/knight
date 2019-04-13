//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Knight.Framework.TinyMode.UI
{
    public class BindableEvent
    {
        public Component        Component       { get; set; }
        public UnityEventBase   UnityEvent      { get; set; }
        public string           Name            { get; set; }
        public Type             DeclaringType   { get; set; }
        public Type             ComponentType   { get; set; }
    }
}
