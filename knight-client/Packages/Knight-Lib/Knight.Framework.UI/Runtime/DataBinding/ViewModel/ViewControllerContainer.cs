using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    public class ViewControllerContainer : MonoBehaviour
    {
        [InfoBox("ViewModelClass can not be null.", InfoBoxType.Error, "IsViewControllerClassNull")]
        [Dropdown("ViewControllerClasses")]
        public string                       ViewControllerClass;

        [ReorderableList]
        [InfoBox("Hei bro!!!!! Some ViewModel has same key.", InfoBoxType.Error, "IsViewModelKeyRepeated")]
        public List<ViewModelDataSource>    ViewModels;
        
        [ReorderableList]
        public List<EventBinding>           EventBindings;

        [HideInInspector]
        public string[]                     ViewControllerClasses = new string[0];

        public void GetAllViewModelDataSources()
        {
            this.ViewModels = new List<ViewModelDataSource>(this.GetComponentsInChildren<ViewModelDataSource>(true));
            this.EventBindings = new List<EventBinding>(this.GetComponentsInChildren<EventBinding>(true));
            this.ViewControllerClasses = DataBindingTypeResolve.GetAllViews().ToArray();
        }

        private bool IsViewControllerClassNull()
        {
            return string.IsNullOrEmpty(this.ViewControllerClass);
        }

        private bool IsViewModelKeyRepeated()
        {
            if (this.ViewModels == null) return false;

            var rTempViewModels = new HashSet<ViewModelDataSource>(this.ViewModels);            
            return rTempViewModels.Count < this.ViewModels.Count;
        }
    }
}
