using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WindHotfix.GUI
{
    public class TViewController<T> : ViewController where T : class
    {
        public List<UnityObject>        Objects;

        protected bool                  mIsOpened       = false;
        protected bool                  mIsClosed       = false;

        public override void Initialize(List<UnityObject> rObjs)
        {
            this.Objects = rObjs;
            
            this.OnInitialize();
        }

        public override bool IsOpened
        {
            get { return mIsOpened; }
            set { mIsOpened = value; }
        }

        public override bool IsClosed
        {
            get { return mIsClosed; }
            set { mIsClosed = value; }
        }

        public override void Opening()
        {
            this.mIsOpened = true;
            this.OnOpening();
        }

        public override void Closing()
        {
            this.mIsClosed = true;
            this.OnClosing();
        }

        public override void Closed()
        {
            this.OnClosed();
        }

        public void EventBinding(UnityEngine.Object rObj, EventTriggerType rType, Action<UnityEngine.Object> rAction)
        {
            HotfixEventManager.Instance.Binding(rObj, rType, rAction);
        }

        public void EventUnBinding(UnityEngine.Object rObj, EventTriggerType rType, Action<UnityEngine.Object> rAction)
        {
            HotfixEventManager.Instance.UnBinding(rObj, rType, rAction);
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void OnOpening()
        {
        }

        public virtual void OnClosing()
        {
        }

        public virtual void OnClosed()
        {
        }
    }
}
