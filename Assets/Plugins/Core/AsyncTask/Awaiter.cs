//======================================================================
//        Forked github: https://github.com/svermeulen/Unity3dAsyncAwaitUtil
//        Email: hgplan@126.com
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Core
{
    public static class Awaiter
    {
        public class CoroutineAwaiter : INotifyCompletion
        {
            private bool        mIsDone;
            private Exception   mException;
            private Action      mContinuation;

            public bool         IsCompelted => mIsDone;

            public void GetResult()
            {
                Assert(mIsDone);

                if (mException != null)
                    throw mException;
            }

            public void Complete(Exception rException)
            {
                Assert(!mIsDone);

                mIsDone = true;
                mException = rException;

                if (mContinuation != null)
                {
                    RunOnUnityScheduler(mContinuation);
                }
            }
            
            void INotifyCompletion.OnCompleted(Action rContinuation)
            {
                Assert(mContinuation == null);
                Assert(!mIsDone);

                mContinuation = rContinuation;
            }
        }

        public class CoroutineAwaiter<T> : INotifyCompletion
        {
            private bool        mIsDone;
            private Exception   mException;
            private Action      mContinuation;
            private T           mResult;

            public bool         IsCompelted => mIsDone;

            public T GetResult()
            {
                Assert(mIsDone);

                if (mException != null)
                    throw mException;

                return mResult;
            }

            public void Complete(T rResult, Exception rException)
            {
                Assert(!mIsDone);

                mIsDone = true;
                mException = rException;
                mResult = rResult;

                if (mContinuation != null)
                {
                    RunOnUnityScheduler(mContinuation);
                }
            }

            void INotifyCompletion.OnCompleted(Action rContinuation)
            {
                Assert(mContinuation == null);
                Assert(!mIsDone);

                mContinuation = rContinuation;
            }
        }

        public static class InstructionWarappers
        {
            public static IEnumerator ReturnSelf<T>(CoroutineAwaiter<T> rAwaiter, T rInstruction)
            {
                yield return rInstruction;
                rAwaiter.Complete(rInstruction, null);
            }

            public static IEnumerator ReturnVoid(CoroutineAwaiter rAwaiter, object rInstruction)
            {
                yield return rInstruction;
                rAwaiter.Complete(null);
            }

            public static IEnumerator AssetBundleCreateRequest(CoroutineAwaiter<AssetBundle> rAwaiter, AssetBundleCreateRequest rInstruction)
            {
                yield return rInstruction;
                rAwaiter.Complete(rInstruction.assetBundle, null);
            }

            public static IEnumerator AssetBundleRequest(CoroutineAwaiter<UnityEngine.Object> rAwaiter, AssetBundleRequest rInstruction)
            {
                yield return rInstruction;
                rAwaiter.Complete(rInstruction.asset, null);
            }

            public static IEnumerator ResourceRequest(CoroutineAwaiter<UnityEngine.Object> rAwaiter, ResourceRequest rInstruction)
            {
                yield return rInstruction;
                rAwaiter.Complete(rInstruction.asset, null);
            }
        }

        class CoroutineWrapper<T>
        {
            private readonly CoroutineAwaiter<T>    mAwaiter;
            private readonly Stack<IEnumerator>     mProcessStack;

            public CoroutineWrapper(IEnumerator rCoroutine, CoroutineAwaiter<T> rAwaiter)
            {
                mProcessStack = new Stack<IEnumerator>();
                mProcessStack.Push(rCoroutine);
                mAwaiter = rAwaiter;
            }

            public IEnumerator Run()
            {
                while (true)
                {
                    var rTopWorker = mProcessStack.Peek();

                    bool bIsDone;

                    try
                    {
                        bIsDone = !rTopWorker.MoveNext();
                    }
                    catch (Exception e)
                    {
                        // The IEnumerators we have in the process stack do not tell us the
                        // actual names of the coroutine methods but it does tell us the objects
                        // that the IEnumerators are associated with, so we can at least try
                        // adding that to the exception output
                        var objectTrace = GenerateObjectTrace(mProcessStack);

                        if (objectTrace.Any())
                        {
                            mAwaiter.Complete(
                                default(T), new Exception(
                                    GenerateObjectTraceMessage(objectTrace), e));
                        }
                        else
                        {
                            mAwaiter.Complete(default(T), e);
                        }

                        yield break;
                    }

                    if (bIsDone)
                    {
                        mProcessStack.Pop();

                        if (mProcessStack.Count == 0)
                        {
                            mAwaiter.Complete((T)rTopWorker.Current, null);
                            yield break;
                        }
                    }

                    // We could just yield return nested IEnumerator's here but we choose to do
                    // our own handling here so that we can catch exceptions in nested coroutines
                    // instead of just top level coroutine
                    if (rTopWorker.Current is IEnumerator)
                    {
                        mProcessStack.Push((IEnumerator)rTopWorker.Current);
                    }
                    else
                    {
                        // Return the current value to the unity engine so it can handle things like
                        // WaitForSeconds, WaitToEndOfFrame, etc.
                        yield return rTopWorker.Current;
                    }
                }
            }

            private string GenerateObjectTraceMessage(List<Type> rObjTrace)
            {
                var rResult = new StringBuilder();

                foreach (var objType in rObjTrace)
                {
                    if (rResult.Length != 0)
                    {
                        rResult.Append(" -> ");
                    }

                    rResult.Append(objType.ToString());
                }

                rResult.AppendLine();
                return "Unity Coroutine Object Trace: " + rResult.ToString();
            }

            private static List<Type> GenerateObjectTrace(IEnumerable<IEnumerator> rEnumerators)
            {
                var rObjTrace = new List<Type>();

                foreach (var rEnumerator in rEnumerators)
                {
                    // NOTE: This only works with scripting engine 4.6
                    // And could easily stop working with unity updates
                    var rField = rEnumerator.GetType().GetField("$this", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                    if (rField == null)
                    {
                        continue;
                    }

                    var rObj = rField.GetValue(rEnumerator);

                    if (rObj == null)
                    {
                        continue;
                    }

                    var rObjType = rObj.GetType();

                    if (!rObjTrace.Any() || rObjType != rObjTrace.Last())
                    {
                        rObjTrace.Add(rObjType);
                    }
                }

                rObjTrace.Reverse();
                return rObjTrace;
            }
        }

        private static void Assert(bool bCond)
        {
            if (!bCond)
            {
                throw new Exception("Assert hit in UnityAysncUtil package!");
            }
        }

        private static void RunOnUnityScheduler(Action rAction)
        {
            if (SynchronizationContext.Current == SyncContextUtil.UnitySynchronizationContext)
            {
                UtilTool.SafeExecute(rAction);
            }
            else
            {
                SyncContextUtil.UnitySynchronizationContext.Post(_ => UtilTool.SafeExecute(rAction), null);
            }
        }

        private static CoroutineAwaiter GetAwaiterReturnVoid(object rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(InstructionWarappers.ReturnVoid(rAwaiter, rInstruction)));
            return rAwaiter;
        }

        private static CoroutineAwaiter<T> GetAwaiterReturnSelf<T>(T rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<T>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(InstructionWarappers.ReturnSelf(rAwaiter, rInstruction)));
            return rAwaiter;
        }

        public static TaskAwaiter GetAwaiter(this TimeSpan rTimeSpan)
        {
            return Task.Delay(rTimeSpan).GetAwaiter();
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForSeconds rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForEndOfFrame rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForFixedUpdate rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitForSecondsRealtime rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitUntil rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter GetAwaiter(this WaitWhile rInstruction)
        {
            return GetAwaiterReturnVoid(rInstruction);
        }

        public static CoroutineAwaiter<AsyncOperation> GetAwaiter(this AsyncOperation rInstruction)
        {
            return GetAwaiterReturnSelf(rInstruction);
        }

        public static CoroutineAwaiter<WWW> GetAwaiter(this WWW rInstruction)
        {
            return GetAwaiterReturnSelf(rInstruction);
        }

        public static CoroutineAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(InstructionWarappers.ResourceRequest(rAwaiter, rInstruction)));
            return rAwaiter;
        }
        
        public static CoroutineAwaiter<AssetBundle> GetAwaiter(this AssetBundleCreateRequest rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<AssetBundle>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(InstructionWarappers.AssetBundleCreateRequest(rAwaiter, rInstruction)));
            return rAwaiter;
        }

        public static CoroutineAwaiter<UnityEngine.Object> GetAwaiter(this AssetBundleRequest rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<UnityEngine.Object>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(InstructionWarappers.AssetBundleRequest(rAwaiter, rInstruction)));
            return rAwaiter;
        }

        public static CoroutineAwaiter<T> GetAwaiter<T>(this IEnumerator<T> rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<T>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(new CoroutineWrapper<T>(rInstruction, rAwaiter).Run()));
            return rAwaiter;
        }

        public static CoroutineAwaiter<object> GetAwaiter(this IEnumerator rInstruction)
        {
            var rAwaiter = new CoroutineAwaiter<object>();
            RunOnUnityScheduler(() => CoroutineManager.Instance.Start(new CoroutineWrapper<object>(rInstruction, rAwaiter).Run()));
            return rAwaiter;
        }
    }
}
