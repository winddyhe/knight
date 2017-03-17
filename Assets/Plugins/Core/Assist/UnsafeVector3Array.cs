//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

namespace Core
{
    /// <summary>
    /// Unsafe Vector3 List 直接操作Array的指针, 避免重复内存GC
    /// 适用于动态Mesh频繁改变顶点的情况
    /// </summary>
    public unsafe class UnsafeVector3Array : IDisposable
    {
        private Vector3[]   mArray;
        private int         mArrayLength;
        private int         mArrayCapacity;

        public unsafe UnsafeVector3Array(int nCapacity)
        {
            mArray = new Vector3[nCapacity];
            mArrayLength = 0;
            mArrayCapacity = nCapacity;
        }

        public void Dispose()
        {
            Clear();
        }

        public unsafe void Add(Vector3 rValue)
        {
            if (mArrayLength >= mArrayCapacity)
            {
                throw new ArgumentOutOfRangeException(string.Format("Array out of Range ! Capacity = {0}", mArrayCapacity));
            }
            fixed (Vector3* p = mArray)
            {
                p[mArrayLength++] = rValue;
            }
        }

        public unsafe void Set(int nIndex, Vector3 rValue)
        {
            fixed (Vector3* p = mArray)
            {
                p[nIndex] = rValue;
            }
        }
        
        public unsafe void Clear()
        {
            mArray = null;
            mArrayLength = 0;
            mArrayCapacity = 0;
        }

        public unsafe void Reset()
        {
            fixed (Vector3* p = mArray)
            {
                for (int i = 0; i < mArrayLength; i++)
                {
                    p[i] = Vector3.zero;
                }
            }
            mArrayLength = 0;
        }

        //public unsafe Vector3[] ToArray()
        //{
        //    fixed (Vector3* p = mArray)
        //    {
        //        return &(Vector3[])(*p);
        //    }
        //}

        public unsafe Vector3 Get(int nIndex)
        {
            fixed (Vector3* p = mArray)
            {
                return p[nIndex];
            }
        }

        public int Length   { get { return mArrayLength; } }
        public int Capacity { get { return mArrayCapacity; } }
    }
}
