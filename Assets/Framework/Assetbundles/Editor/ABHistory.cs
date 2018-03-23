//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Core;
using System.IO;
using UnityEngine.AssetBundles;

namespace UnityEditor.AssetBundles
{
    public class ABHistory
    {
        public class Entry
        {
            public string               Time;
            public string               Path;
            public ABVersion            IncVer;
            public bool                 IsSelected;
        }

        private Dict<string, Entry>     mEntries;
        public Dict<string, Entry>      Entries { get { return mEntries; } }

        public void Initialize(string rOutPath)
        {
            mEntries = new Dict<string, Entry>();
            
            string rHistoryPath = UtilTool.PathCombine(rOutPath, "History");
            DirectoryInfo rHistoryDirInfo = new DirectoryInfo(rHistoryPath);
            if (!rHistoryDirInfo.Exists) return;

            ABVersion rCurVersion = ABVersionEditor.Load(rOutPath);

            var rSubDirs = rHistoryDirInfo.GetDirectories();
            for (int i = 0; i < rSubDirs.Length; i++)
            {
                Entry rEntry = new Entry();
                rEntry.Path = UtilTool.PathCombine(rHistoryPath, rSubDirs[i].Name, ABVersion.ABVersion_File_Bin);
                rEntry.Time = rSubDirs[i].Name;
                rEntry.IncVer = new ABVersion();
                rEntry.IncVer.Entries = new Dict<string, ABVersionEntry>();

                ABVersion rHistoryVersion = ABVersionEditor.Load(rSubDirs[i].FullName);

                // 比较两个版本
                foreach (var rPair in rCurVersion.Entries)
                {
                    var rAVEntry = rPair.Value;
                    var rOldEntry = rHistoryVersion.GetEntry(rAVEntry.Name);
                    if (rOldEntry == null)  // 说明是新增的
                    {
                        rEntry.IncVer.Entries.Add(rAVEntry.Name, rAVEntry);
                    }
                    else
                    {
                        if (rOldEntry.Version != rAVEntry.Version)  // 版本不一致
                        {
                            rEntry.IncVer.Entries.Add(rAVEntry.Name, rAVEntry);
                        }
                    }
                }
                mEntries.Add(rEntry.Time, rEntry);
            }
        }
    }
}
