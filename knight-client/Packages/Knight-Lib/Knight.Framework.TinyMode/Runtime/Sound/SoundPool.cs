//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.TinyMode
{
    public class SoundPool
    {
        /// <summary>
        /// 缓存，这个和rootGo节点下的对象个数一一对应
        /// </summary>
        private TObjectPool<AudioSource>    mObjectPool;
        /// <summary>
        /// 对象池的根节点
        /// </summary>
        private GameObject                  mRootGo;
        /// <summary>
        /// 对象池的根节点
        /// </summary>
        public  GameObject                  RootGo { get { return mRootGo; } }

        public SoundPool(string rPoolName, int rInitCount = 0)
        {
            this.mObjectPool = new TObjectPool<AudioSource>(OnAlloc, OnFree, OnDestroy);

            this.mRootGo = UtilTool.CreateGameObject(rPoolName);
            this.mRootGo.transform.position = new Vector3(0, 0, 0);

            for (int i = 0; i < rInitCount; i++)
            {
                this.mObjectPool.Alloc();
            }
        }

        public AudioSource Alloc()
        {
            return this.mObjectPool.Alloc();
        }

        public void Free(AudioSource rAudioSource)
        {
            if (rAudioSource == null) return;
            this.mObjectPool.Free(rAudioSource);
        }

        public void Destroy()
        {
            this.mObjectPool.Destroy();
            UtilTool.SafeDestroy(this.mRootGo);
        }

        private AudioSource OnAlloc()
        {
            return this.mRootGo.AddComponent<AudioSource>();
        }

        private void OnFree(AudioSource rAudioSource)
        {
        }

        private void OnDestroy(AudioSource rAudioSource)
        {
            UtilTool.SafeDestroy(rAudioSource);
        }
    }
}
