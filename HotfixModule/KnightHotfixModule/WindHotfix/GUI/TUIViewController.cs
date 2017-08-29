using Framework.Hotfix;
using System;
using System.Collections.Generic;

namespace WindHotfix.GUI
{
    public class TUIViewController<T> : UIViewController where T : class
    {
        public List<UnityObject>        Objects;
        public List<BaseDataObject>     BaseDatas;

        protected HotfixEventHandler    mEventHandler;
        protected bool                  mIsOpened       = false;
        protected bool                  mIsClosed       = false;

        public override void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.Objects = rObjs;
            this.BaseDatas = rBaseDatas;

            this.mEventHandler = new HotfixEventHandler();
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
            if (mEventHandler != null)
                mEventHandler.RemoveAll();
            mEventHandler = null;

            this.OnClosed();
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            if (mEventHandler == null) return;
            mEventHandler.Handle(rTarget);
        }

        public object GetData(string rName)
        {
            if (this.BaseDatas == null) return null;
            var rBaseDataObj = this.BaseDatas.Find((rItem) => { return rItem.Name.Equals(rName); });
            if (rBaseDataObj == null) return null;
            return rBaseDataObj.Object;
        }

        public void AddEventListener(UnityEngine.Object rObj, Action<UnityEngine.Object> rAction)
        {
            if (this.mEventHandler == null) return;
            this.mEventHandler.AddEventListener(rObj, rAction);
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
