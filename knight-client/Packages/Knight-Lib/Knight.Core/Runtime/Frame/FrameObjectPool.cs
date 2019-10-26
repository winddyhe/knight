using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Core
{
    public class FrameObject : AsyncRequest<FrameObject>
    {
        public int          Index;
        public GameObject   TemplateGo;
        public GameObject   ResultGo;
        public bool         IsInstanced;
    }

    public class FrameObjectPool
    {
        private TObjectPool<FrameObject> mObjectPool;

        public FrameObjectPool()
        {
            this.mObjectPool = new TObjectPool<FrameObject>(this.OnAlloc, this.OnFree, this.OnDestroy);
        }

        public FrameObject Alloc(int nIndex, GameObject rTemplateGo)
        {
            var rFrameObject = this.mObjectPool.Alloc();
            rFrameObject.Index = nIndex;
            rFrameObject.TemplateGo = rTemplateGo;
            rFrameObject.ResultGo = null;
            rFrameObject.IsInstanced = false;
            return rFrameObject;
        }

        public void Free(FrameObject rFrameObject)
        {
            this.mObjectPool.Free(rFrameObject);
        }

        public void Destroy()
        {
            this.mObjectPool.Destroy();
        }

        private FrameObject OnAlloc()
        {
            return this.mObjectPool.Alloc();
        }

        private void OnFree(FrameObject rFrameObject)
        {
            rFrameObject.Index = -1;
            rFrameObject.TemplateGo = null;
            rFrameObject.ResultGo = null;
            rFrameObject.IsInstanced = false;
        }

        private void OnDestroy(FrameObject rFrameObject)
        {
            rFrameObject.Index = -1;
            rFrameObject.TemplateGo = null;
            rFrameObject.ResultGo = null;
            rFrameObject.IsInstanced = false;
        }
    }
}
