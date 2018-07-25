using System;
using System.Collections.Generic;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class EventBinding : MonoBehaviour
    {
        [Dropdown("ViewEvents")]
        public string       ViewEvent;
        [Dropdown("ViewModelMethods")]
        public string       ViewModelMethod;
    }

    public partial class EventBinding
    {
        [HideInInspector]
        public string[]     ViewEvents          = new string[0];
        [HideInInspector]
        public string[]     ViewModelMethods    = new string[0];

        public void GetPaths()
        {
            this.ViewEvents = DataBindingTypeResolve.GetBindableEventPaths(this.gameObject);
            this.ViewModelMethods = DataBindingTypeResolve.GetViewModelBindingEvents(this.gameObject);
        }
    }
}
