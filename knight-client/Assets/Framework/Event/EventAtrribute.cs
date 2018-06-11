//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Framework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAtrribute : Attribute
    {
        public ushort  MsgCode;

        public EventAtrribute(ushort nMsgCode)
        {
            this.MsgCode = nMsgCode;
        }
    }
}
