using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    public class ViewModelContainer : MonoBehaviour
    {
        [InfoBox("ViewModelClass can not be null.", InfoBoxType.Error, "IsViewModelClassNull")]
        public string                       ViewModelClass;

        [ReorderableKeyList]
        [InfoBox("Hei bro!!!!! Some ViewModel has same key.", InfoBoxType.Error, "IsViewModelKeyRepeated")]
        public List<ViewModelDataSource>    ViewModels;

        private bool IsViewModelClassNull()
        {
            return string.IsNullOrEmpty(this.ViewModelClass);
        }

        private bool IsViewModelKeyRepeated()
        {
            if (this.ViewModels == null) return false;

            HashSet<string> rKeys = new HashSet<string>();
            for (int i = 0; i < this.ViewModels.Count; i++)
            {
                var rKey = this.ViewModels[i].Key;
                rKeys.Add(rKey);
            }
            return rKeys.Count < this.ViewModels.Count;
        }
    }
}
