//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;

namespace Core
{
    public static class ObjectExpand
    {
        /************************************************************************************
         * Component的方法扩展
         *      ReceiveComponent
         *      SafeGetComponent
         ************************************************************************************/
        public static T SafeGetComponent<T>(this GameObject rGo) where T : Component
        {
            if (rGo == null) return default(T);
            return rGo.GetComponent<T>();
        }

        public static T ReceiveComponent<T>(this GameObject rGo) where T : Component
        {
            if (rGo == null) return default(T);

            T rComponent = rGo.GetComponent<T>();
            if (rComponent == null) rComponent = rGo.AddComponent<T>();

            return rComponent;
        }
    }
}


