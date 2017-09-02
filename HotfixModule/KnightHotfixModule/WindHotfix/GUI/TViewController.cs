using Framework.Hotfix;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace WindHotfix.GUI
{
    public class TViewController<T> : ViewController where T : class
    {
        public List<UnityObject>        Objects;
        public List<BaseDataObject>     BaseDatas;

        protected bool                  mIsOpened       = false;
        protected bool                  mIsClosed       = false;

        public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.Objects = rObjs;
            this.BaseDatas = rBaseDatas;
            
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

        public object GetData(string rName)
        {
            if (this.BaseDatas == null) return null;
            var rBaseDataObj = this.BaseDatas.Find((rItem) => { return rItem.Name.Equals(rName); });
            if (rBaseDataObj == null) return null;
            return rBaseDataObj.Object;
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
