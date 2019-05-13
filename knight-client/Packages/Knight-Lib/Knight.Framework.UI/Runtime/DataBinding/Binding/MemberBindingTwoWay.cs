using System;
using System.Collections.Generic;
using Knight.Core;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [DefaultExecutionOrder(100)]
    public partial class MemberBindingTwoWay : MemberBindingAbstract
    {
        [Dropdown("EventPaths")]
        [DrawOrder(2)]
        public  string              EventPath;

        private UnityEventWatcher   mUnityEventWatcher;

        private void Awake()
        {
            this.IsDataConvert = false;
        }

        public void InitEventWatcher()
        {
            var rBoundEvent = DataBindingTypeResolve.MakeViewDataBindingEvent(this.gameObject, this.EventPath);
            if (rBoundEvent != null)
            {
                this.mUnityEventWatcher = new UnityEventWatcher(rBoundEvent.Component, rBoundEvent.Name, this.SyncFromView);
            }
            else
            {
                Debug.LogErrorFormat("Can not parse bound event: {0}.", this.EventPath);
            }
        }

        public override void OnDestroy()
        {
            this.mUnityEventWatcher?.Dispose();
        }

        private void SyncFromView(EventArg rEventArg)
        {
            this.SyncFromView();
        }
    }

    public partial class MemberBindingTwoWay
    {
        [HideInInspector]
        public string[]             EventPaths = new string[0];

        public void GetEventPaths()
        {
            this.EventPaths = DataBindingTypeResolve.GetBindableEventPaths(this.gameObject);
            if (string.IsNullOrEmpty(this.EventPath))
            {
                this.EventPath = this.EventPaths.Length > 0 ? this.EventPaths[0] : "";
            }
        }
    }
}
