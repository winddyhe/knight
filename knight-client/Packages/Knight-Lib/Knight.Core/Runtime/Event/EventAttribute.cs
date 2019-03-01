//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Core
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute
    {
        public ushort  MsgCode;

        public EventAttribute(ushort nMsgCode)
        {
            this.MsgCode = nMsgCode;
        }
    }
}
