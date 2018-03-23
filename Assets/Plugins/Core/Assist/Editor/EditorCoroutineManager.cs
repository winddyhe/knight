//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Core.Editor
{
    /// <summary>
    /// Editor下的协程管理器
    /// </summary>
    public static class EditorCoroutineManager
    {
        /// <summary>
        /// Editor下的一个协程
        /// </summary>
        private class EditorCoroutine : IEnumerator
        {
            private Stack<IEnumerator> mExecutionStack;
    
            public EditorCoroutine(IEnumerator rIter)
            {
                this.mExecutionStack = new Stack<IEnumerator>();
                this.mExecutionStack.Push(rIter);
            }
    
            public object Current
            {
                get { return this.mExecutionStack.Peek().Current; }
            }
    
            public bool MoveNext()
            {
                IEnumerator i = this.mExecutionStack.Peek();
                if (i.MoveNext())
                {
                    object rResult = i.Current;
                    if (rResult != null && rResult is IEnumerator)
                    {
                        this.mExecutionStack.Push((IEnumerator)rResult);
                    }
                    return true;
                }
                else
                {
                    if (this.mExecutionStack.Count > 1)
                    {
                        this.mExecutionStack.Pop();
                        return true;
                    }
                }
                return false;
            }
    
            public void Reset()
            {
                throw new System.NotImplementedException();
            }
    
            public bool Find(IEnumerator rIter)
            {
                return this.mExecutionStack.Contains(rIter);
            }
        }
    
        
        /*****************************************************************/
        /*****************************************************************/
    
        private static List<EditorCoroutine> mEditorCoroutines;
    
        private static List<IEnumerator>     mBuffers;
    
        /// <summary>
        /// 开始一个协程
        /// </summary>
        public static IEnumerator Start(IEnumerator rIter)
        {
            if (mEditorCoroutines == null)
                mEditorCoroutines = new List<EditorCoroutine>();
            
            if (mBuffers == null)
                mBuffers = new List<IEnumerator>();
    
            if (mEditorCoroutines.Count == 0)
            {
                EditorApplication.update += Update;
            }
    
            mBuffers.Add(rIter);
    
            return rIter;
        }
    
        private static bool Find(IEnumerator rIter)
        {
            foreach (var rEditorCoroutine in mEditorCoroutines)
    	    {
                if (rEditorCoroutine.Find(rIter))
                    return true;
    	    }
            return false;
        }
    
        /// <summary>
        /// 更新
        /// </summary>
        private static void Update()
        {
            mEditorCoroutines.RemoveAll(rCoroutine => { return rCoroutine.MoveNext() == false; });
    
            if (mBuffers.Count > 0)
            {
                foreach (var rIter in mBuffers)
                {
                    if (!Find(rIter))
                        mEditorCoroutines.Add(new EditorCoroutine(rIter));
                }
                mBuffers.Clear();
            }
    
            if (mEditorCoroutines.Count == 0)
                EditorApplication.update -= Update;
        }
    }
}