//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{    
    [DefaultExecutionOrder(90)]
    [System.Serializable]
    public class ViewModelDataSource : MonoBehaviour
    {
        [InfoBox("ViewClass can not be null.", InfoBoxType.Error, "IsKeyNull")]
        public string   Key;

        [Dropdown("ViewModelClasses")]
        public string   ViewModelPath;

        [HideInInspector]
        public string[] ViewModelClasses = new string[0];

        private bool IsKeyNull()
        {
            return string.IsNullOrEmpty(this.Key);
        }

        public void GetPaths()
        {
            this.ViewModelClasses = DataBindingTypeResolve.GetAllViewModels().ToArray();
        }
    }
}