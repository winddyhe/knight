using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityFx.Async;

namespace Knight.Core
{
    public class FrameInstancer : TSingleton<FrameInstancer>
    {
        private int                 mCountIndex = 0;
        private Queue<FrameObject>  mInstanceQueue;
        private FrameObjectPool     mFrameObjectPool;
        
        private FrameInstancer()
        {
        }

        public void Initialize()
        {
            this.mCountIndex = 0;
            this.mFrameObjectPool = new FrameObjectPool();
            this.mInstanceQueue = new Queue<FrameObject>();
        }

        public async Task<GameObject> Instantiate(GameObject rTemplateGo)
        {
            var rFrameObj = await this.InstantiateAsync(rTemplateGo);

            if (rFrameObj == null) return null;
            var rResultGo = rFrameObj.ResultGo;
            this.mFrameObjectPool.Free(rFrameObj);

            return rResultGo;
        }

        public void Destroy()
        {
            this.mCountIndex = 0;
            this.mInstanceQueue.Clear();
            this.mFrameObjectPool.Destroy();
        }

        public void Update(float fDeltaTime)
        {
            if (this.mInstanceQueue == null || this.mInstanceQueue.Count == 0) return;

            var rFrameObj = this.mInstanceQueue.Dequeue();
            rFrameObj.ResultGo = GameObject.Instantiate(rFrameObj.TemplateGo);
            rFrameObj.IsInstanced = true;
        }

        private IAsyncOperation<FrameObject> InstantiateAsync(GameObject rTemplateGo)
        {
            var rFrameObj = this.mFrameObjectPool.Alloc(this.mCountIndex++, rTemplateGo);
            this.mInstanceQueue.Enqueue(rFrameObj);
            return rFrameObj.Start(this.InstantiateAsync(rFrameObj));
        }

        private IEnumerator InstantiateAsync(FrameObject rFrameObj)
        {
            while(!rFrameObj.IsInstanced)
            {
                yield return 0;
            }
            rFrameObj.SetResult(rFrameObj);
        }
    }
}
