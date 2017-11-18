//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WindHotfix.Game
{
    public class GameSystem
    {
        public    bool        IsActive;
        protected GCContainer mResultComps;

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public virtual void Stop()
        {
        }
    }
}