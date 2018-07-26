using System;
using System.Collections.Generic;
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

        public void InitEventWatcher()
        {
            var rBoundEvent = DataBindingTypeResolve.MakeViewDataBindingEvent(this.gameObject, this.EventPath);
            this.mUnityEventWatcher = new UnityEventWatcher(rBoundEvent.Component, rBoundEvent.Name, this.SyncFromView);
        }

        public override void OnDestroy()
        {
            this.mUnityEventWatcher?.Dispose();
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
