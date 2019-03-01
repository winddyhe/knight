using System;
using System.Collections.Generic;
using Knight.Core;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class EventBinding : MonoBehaviour
    {
        public bool                 IsListTemplate;
        [Dropdown("ViewEvents")]
        public  string              ViewEvent;
        [Dropdown("ViewModelMethods")]
        public  string              ViewModelMethod;
        
        private UnityEventWatcher   mUnityEventWatcher;

        public void InitEventWatcher(Action<EventArg> rAction)
        {
            var rBoundEvent = DataBindingTypeResolve.MakeViewDataBindingEvent(this.gameObject, this.ViewEvent);
            if (rBoundEvent != null)
            {
                this.mUnityEventWatcher = new UnityEventWatcher(rBoundEvent.Component, rBoundEvent.Name, rAction);
            }
            else
            {
                Debug.LogErrorFormat("Can not parse bound event: {0}.", this.ViewEvent);
            }
        }

        public void OnDestroy()
        {
            this.mUnityEventWatcher?.Dispose();
        }
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

            if (string.IsNullOrEmpty(this.ViewEvent))
            {
                this.ViewEvent = this.ViewEvents.Length > 0 ? this.ViewEvents[0] : "";
            }
            if (string.IsNullOrEmpty(this.ViewModelMethod))
            {
                this.ViewModelMethod = this.ViewModelMethods.Length > 0 ? this.ViewModelMethods[0] : "";
            }
        }
    }
}
