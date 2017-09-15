//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Core;

namespace Framework.Graphics
{
    public class VertexListPool<T>
    {
        private static readonly TObjectPool<List<T>> mListPool = new TObjectPool<List<T>>(OnAlloc, OnDestroy, OnDestroy);

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

        private static List<T> OnAlloc()
        {
            return new List<T>();
        }

        private static void OnDestroy(List<T> rList)
        {
            rList.Clear();
        }
    }
}
