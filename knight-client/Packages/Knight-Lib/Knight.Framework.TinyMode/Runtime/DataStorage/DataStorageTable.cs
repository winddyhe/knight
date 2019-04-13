//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using Knight.Core.WindJson;
using UnityEngine;

namespace Knight.Framework.TinyMode
{
    public interface IDataStorageTable
    {
        string TableName { get; set; }
        void Load();
        void Save();
    }

    public class TDataStorageTable<T> : IDataStorageTable where T : class, new()
    {
        private const string    kConstDataTablePrefix   = "DATA_STORAGE_";

        public string           TableName               { get; set; }
        public T                Value                   { get; set; }
        
        public void Load()
        {
            var rValue = PlayerPrefs.GetString(kConstDataTablePrefix + this.TableName, "");
            if (!string.IsNullOrEmpty(rValue))
            {
                try
                {
                    var rJsonNode = JsonParser.Parse(rValue);
                    this.Value = rJsonNode.ToObject(typeof(T)) as T;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    this.Value = new T();
                }
            }
            else
            {
                this.Value = new T();
            }
        }

        public void Save()
        {
            if (this.Value == null)
            {
                PlayerPrefs.SetString(kConstDataTablePrefix + this.TableName, "");
            }
            else
            {
                var rJsonNode = JsonParser.ToJsonNode(this.Value);
                PlayerPrefs.SetString(kConstDataTablePrefix + this.TableName, rJsonNode.ToString());
            }
            PlayerPrefs.Save();
        }
    }
}
