using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Async;

namespace Knight.Core
{
    public class AsyncRequest<T> where T : class
    {
        private CoroutineHandler            mHandler;
        private AsyncCompletionSource<T>    mAsyncResult;

        public IAsyncOperation<T> Start(IEnumerator rIEnum, Action<float> rProgressAction = null)
        {
            this.mAsyncResult = new AsyncCompletionSource<T>();
            this.mHandler = CoroutineManager.Instance.StartHandler(rIEnum);
            this.mAsyncResult.AddCompletionCallback((rContinuation) =>
            {
                if (rContinuation.IsCanceled || rContinuation.IsFaulted)
                {
                    CoroutineManager.Instance.Stop(this.mHandler);
                }
            });
            if (rProgressAction != null)
            {
                this.mAsyncResult.AddProgressCallback(rProgressAction);
            }
            return this.mAsyncResult.Operation;
        }

        public void SetResult(T rResult)
        {
            if (this.mAsyncResult != null)
                this.mAsyncResult.SetResult(rResult);
        }

        public void Stop()
        {
            if (this.mAsyncResult == null)
            {
                if (this.mHandler != null)
                {
                    CoroutineManager.Instance.Stop(this.mHandler);
                }
            }
            else
            {
                this.mAsyncResult.Cancel();
            }
        }
    }

    public static class WaitAsync
    {
        public class WaitForEndOfFrameRequest : AsyncRequest<WaitForEndOfFrameRequest>
        {
        }

        public class WaitForSecondsRequest : AsyncRequest<WaitForSecondsRequest>
        {
            public float Time;

            public WaitForSecondsRequest(float fTime)
            {
                this.Time = fTime;
            }
        }

        public class IEnumeratorRequest : AsyncRequest<IEnumeratorRequest>
        {
        }

        public static IAsyncOperation<WaitForEndOfFrameRequest> WaitForEndOfFrame()
        {
            var rRequest = new WaitForEndOfFrameRequest();
            return rRequest.Start(WaitForEndOfFrame_Async(rRequest));
        }

        public static IAsyncOperation<WaitForSecondsRequest> WaitForSeconds(float fTime)
        {
            var rRequest = new WaitForSecondsRequest(fTime);
            return rRequest.Start(WaitForSeconds_Async(rRequest));
        }

        private static IEnumerator WaitForEndOfFrame_Async(WaitForEndOfFrameRequest rRequest)
        {
            yield return new WaitForEndOfFrame();
            rRequest.SetResult(rRequest);
        }

        private static IEnumerator WaitForSeconds_Async(WaitForSecondsRequest rRequest)
        {
            yield return new WaitForSeconds(rRequest.Time);
            rRequest.SetResult(rRequest);
        }

        public static async void WarpErrors(this Task rTask)
        {
            await rTask;
        }
    }
}
