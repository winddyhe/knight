//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Core;
using Core.WindJson;

namespace UnityEngine.AssetBundles
{

    public class ABUpdater : TSingleton<ABUpdater>
    {
        public string       mStreamingMD5;
        public string       mPersistentMD5;
        public string       mServerMD5;

        public ABVersion    mStreamingVersion;
        public ABVersion    mPersistentVersion;
        public ABVersion    mServerVersion;

        private ABUpdater()
        {
        }

        //public IEnumerator Initialize()
        //{
        //}
    }
}
