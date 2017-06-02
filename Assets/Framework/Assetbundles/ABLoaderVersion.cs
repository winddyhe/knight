//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Core;

namespace UnityEngine.AssetBundles
{
    public enum LoaderSpace
    {
        Streaming,
        Persistent,
        Server,
        Editor,
    }

    public class ABLoaderEntry
    {
        public ABVersionEntry               VersionEntry;
        public LoaderSpace                  LoaderSpace;
    }

    public class ABLoaderVersion : TSingleton<ABLoaderVersion>
    {
        public Dict<string, ABLoaderEntry>  Entries;

        private ABLoaderVersion()
        {
            this.Entries = new Dict<string, ABLoaderEntry>();
        }
    }
}
