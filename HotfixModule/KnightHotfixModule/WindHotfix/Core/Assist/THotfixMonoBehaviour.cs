using Framework.Hotfix;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace WindHotfix.Core
{
    public class THotfixMonoBehaviour<T> where T : class
    {
        public List<UnityObject>        Objects;
        public List<BaseDataObject>     BaseDatas;

        protected HotfixEventHandler    mEventHandler;

        public void Initialize(List<UnityObject> rObjs, List<BaseDataObject> rBaseDatas)
        {
            this.Objects   = rObjs;
            this.BaseDatas = rBaseDatas;

            this.mEventHandler = new HotfixEventHandler();
            this.OnInitialize();
        }

        public virtual void OnInitialize()
        {
        }

        public virtual void Awake()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        /// <summary>
        /// @TODO: 这样子做可能有风险，无法执行到OnDestroy导致mEventHandler的引用计数不对
        ///        等框架完善之后再做改进
        /// </summary>
        public void Destroy()
        {
            if (mEventHandler != null)
                mEventHandler.RemoveAll();
            mEventHandler = null;

            this.OnDestroy();
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public void OnUnityEvent(Object rTarget)
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
    }
}
