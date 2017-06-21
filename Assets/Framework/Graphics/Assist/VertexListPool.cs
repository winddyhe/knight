using System;
using System.Collections.Generic;
using Core;

namespace Framework.Graphics
{
    public class VertexListPool<T>
    {
        private static readonly TObjectPool<List<T>> mListPool = new TObjectPool<List<T>>(null, OnDestroy, OnDestroy);

        public static List<T> Alloc()
        {
            return mListPool.Alloc();
        }

        public static void Free(List<T> toRelease)
        {
            mListPool.Free(toRelease);
        }

        public static void Destroy()
        {
            mListPool.Destroy();
        }

        private static void OnDestroy(List<T> rList)
        {
            rList.Clear();
        }
    }
}
