//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            GameObject.DontDestroyOnLoad(this);
        }
    }
}
