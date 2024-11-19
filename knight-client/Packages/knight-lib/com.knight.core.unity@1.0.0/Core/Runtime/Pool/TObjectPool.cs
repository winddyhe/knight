using System;
using System.Collections.Generic;

namespace Knight.Core
{
    public class TObjectPool<T> where T : new()
    {
        private Stack<T> mStack;

        private Func<T> mActionAlloc;
        private Action<T> mActionFree;
        private Action<T> mActionDestroy;

        public int AllocCount;
        public int FreeCount;

        public TObjectPool(Func<T> rActionAlloc, Action<T> rActionFree, Action<T> rActionDestroy)
        {
            this.mStack = new Stack<T>();

            this.mActionAlloc = rActionAlloc;
            this.mActionFree = rActionFree;
            this.mActionDestroy = rActionDestroy;

            this.AllocCount = 0;
            this.FreeCount = 0;
        }

        public T Alloc()
        {
            if (this.mStack.Count == 0)
            {
                this.AllocCount++;
                return UtilUnityTool.SafeExecute(this.mActionAlloc);
            }
            else
            {
                var ret = this.mStack.Pop();
                this.AllocCount++;
                this.FreeCount--;
                return ret;
            }
        }

        public void Free(T rElement)
        {
            if (this.mStack.Count > 0 && object.ReferenceEquals(this.mStack.Peek(), rElement)) return;

            if (this.mActionFree != null)
                this.mActionFree(rElement);
            this.AllocCount--;
            this.FreeCount++;
            this.mStack.Push(rElement);
        }

        public void Destroy()
        {
            if (this.mStack == null) return;

            foreach (var rItem in this.mStack)
            {
                UtilUnityTool.SafeExecute(this.mActionDestroy, rItem);
            }
            this.mStack.Clear();
            this.AllocCount = 0;
            this.FreeCount = 0;
        }
    }
}
