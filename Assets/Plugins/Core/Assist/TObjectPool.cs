using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class TObjectPool<T> where T : new()
    {
        private Stack<T> mStack;

        private Action<T> mActionAlloc;
        private Action<T> mActionFree;
        private Action<T> mActionDestroy;

        public TObjectPool(Action<T> rActionAlloc, Action<T> rActionFree, Action<T> rActionDestroy)
        {
            mStack = new Stack<T>();

            mActionAlloc = rActionAlloc;
            mActionFree  = rActionFree;
            mActionDestroy = rActionDestroy;
        }

        public T Alloc()
        {
            T rElement = default(T);
            if (mStack.Count == 0)
            {
                rElement = new T();
            }
            else
            {
                rElement = mStack.Pop();
            }
            UtilTool.SafeExecute(mActionAlloc, rElement);
            return rElement;
        }

        public void Free(T rElement)
        {
            if (mStack.Count > 0 && object.ReferenceEquals(mStack.Peek(), rElement))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");

            if (mActionFree != null)
                mActionFree(rElement);

            mStack.Push(rElement);
        }

        public void Destroy()
        {
            if (mStack == null) return;
            foreach (var rItem in mStack)
            {
                UtilTool.SafeExecute(mActionDestroy, rItem);
            }
            mStack.Clear();
        }
    }
}
