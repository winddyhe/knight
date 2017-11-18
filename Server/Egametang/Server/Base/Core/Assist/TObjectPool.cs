using System;
using System.Collections.Generic;

namespace Core
{
    public class TObjectPool<T> where T : new()
    {
        private Stack<T>    mStack;

        private Func<T>     mActionAlloc;
        private Action<T>   mActionFree;
        private Action<T>   mActionDestroy;

        public TObjectPool(Func<T> rActionAlloc, Action<T> rActionFree, Action<T> rActionDestroy)
        {
            mStack = new Stack<T>();

            mActionAlloc = rActionAlloc;
            mActionFree  = rActionFree;
            mActionDestroy = rActionDestroy;
        }

        public T Alloc()
        {
            if (mStack.Count == 0)
            {
                return UtilTool.SafeExecute(mActionAlloc);
            }
            else
            {
                return mStack.Pop();
            }
        }

        public void Free(T rElement)
        {
            if (mStack.Count > 0 && object.ReferenceEquals(mStack.Peek(), rElement))
                Model.Log.Debug("Internal error. Trying to destroy object that is already released to pool.");

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
