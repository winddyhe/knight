//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.TinyMode
{
    public class DataStorageManager : TSingleton<DataStorageManager>
    {
        public Dict<string, IDataStorageTable> mDataTables;

        private DataStorageManager()
        {
        }

        public void Initialize()
        {
            this.mDataTables = new Dict<string, IDataStorageTable>();
        }

        public void AddTable(string rTableName, IDataStorageTable rTable)
        {
            if (this.mDataTables.ContainsKey(rTableName))
            {
                Debug.LogError($"Data table is repeat: {rTableName}.");
                return;
            }
            rTable.TableName = rTableName;
            this.mDataTables.Add(rTableName, rTable);
        }

        public void LoadAll()
        {
            foreach (var rPair in this.mDataTables)
            {
                rPair.Value.Load();
            }
        }

        public void Save(string rTableName)
        {
            IDataStorageTable rTable = null;
            if (this.mDataTables.TryGetValue(rTableName, out rTable))
            {
                rTable.Save();
            }
        }

        public void SaveAll()
        {
            foreach (var rPair in this.mDataTables)
            {
                rPair.Value.Save();
            }
        }
    }
}
