//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.TinyMode
{
    public class SoundManager : MonoBehaviour
    {
        private static SoundManager __instance;
        public  static SoundManager Instance { get { return __instance; } }

        public  AudioSource         AudioSource;
        public  SoundPool           AudoiSourcePool;

        public  bool                IsSoundOn;

        void Awake()
        {
            if (__instance == null)
                __instance = this;
        }

        public void Initialize()
        {
            this.AudoiSourcePool = new SoundPool("__sound_pool_root");
        }

        public void PlaySingle(string rAudioName)
        {
            if (!this.IsSoundOn) return;

            var rAudioClip = SoundAssetLoader.Instance.GetAudioClip(rAudioName);
            if (rAudioClip == null)
            {
                Debug.LogErrorFormat("Cannot find audio name: ", rAudioName);
                return;
            }
            
            this.AudioSource.Stop();

            this.AudioSource.clip = rAudioClip;
            this.AudioSource.Play();
        }

        public void PlaySingle(string rAudioName, bool bIsOverride)
        {
            if (!this.IsSoundOn) return;

            var rAudioClip = SoundAssetLoader.Instance.GetAudioClip(rAudioName);
            if (rAudioClip == null)
            {
                Debug.LogErrorFormat("Cannot find audio name: ", rAudioName);
                return;
            }

            if (this.AudioSource.isPlaying && bIsOverride)
            {
                Debug.LogError(rAudioName);
                this.AudioSource.clip = rAudioClip;
                this.AudioSource.Stop();
                this.AudioSource.Play();
            }
        }

        public void PlayMulti(string rAudioName)
        {
            if (!this.IsSoundOn) return;

            var rAudioClip = SoundAssetLoader.Instance.GetAudioClip(rAudioName);
            if (rAudioClip == null)
            {
                Debug.LogErrorFormat("Cannot find audio name: ", rAudioName);
                return;
            }
            CoroutineManager.Instance.Start(this.PlayMultiAsync(rAudioClip));
        }

        private IEnumerator PlayMultiAsync(AudioClip rAudioClip)
        {
            var rAudioSource = this.AudoiSourcePool.Alloc();
            rAudioSource.clip = rAudioClip;

            rAudioSource.Stop();
            rAudioSource.Play();

            yield return new WaitForSeconds(rAudioClip.length + 0.1f);

            rAudioSource.clip = null;
            rAudioSource.Stop();
            this.AudoiSourcePool.Free(rAudioSource);
        }
    }
}
