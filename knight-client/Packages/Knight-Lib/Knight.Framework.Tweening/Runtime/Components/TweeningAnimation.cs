using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Knight.Core;

namespace Knight.Framework.Tweening
{
    public class TweeningAnimation : MonoBehaviour
    {
        public    bool            IsLoop;
        public    LoopType        LoopType;
        public    int             LoopCount;
                  
        public    AnimationCurve  TimeCurve;
        public    float           Duration;
        public    float           Delay;
                  
        public    bool            IsIgnoreTimeScale;
        public    bool            IsUseFixedUpdate;
        public    bool            IsAutoExecute;
        
        public    TweenCallback   OnPlayCompleted;
        
        protected Tweener         mTweener;
        
        protected void OnEnable()
        {
            this.CreateTweener();
        }

        protected void CreateTweener()
        {
            // 创建一个Tweener对象
            this.OnCreateTweener();

            if (this.mTweener == null)
            {
                Debug.LogError("Create tweener failed.");
                return;
            }
            // 构建Tweener的各种参数
            this.SetUpTweener();

            // 是否自动执行 在Awake的时候
            if (this.IsAutoExecute)
            {
                this.mTweener.Play();
            }
        }

        protected virtual void OnDestroy()
        {
            this.Stop();
        }
        
        public void SetCompleted(Action rPlayCompleted)
        {
            this.OnPlayCompleted = ()=> { UtilTool.SafeExecute(rPlayCompleted); };
            this.mTweener.OnComplete(this.OnPlayCompleted);
        }

        public void Play()
        {
            if (this.mTweener == null)return;
            this.mTweener.Play();
        }

        public void Stop()
        {
            if (this.mTweener == null) return;
            this.mTweener.Kill(false);
        }

        public void Pause()
        {
            if (this.mTweener == null) return;
            this.mTweener.Pause();
        }
        
        private void SetUpTweener()
        {
            if (this.mTweener == null) return;

            // 先暂停
            this.mTweener.Pause();
            this.mTweener.SetUpdate(this.IsUseFixedUpdate ? UpdateType.Fixed : UpdateType.Normal, false);
            this.mTweener.timeScale = this.IsIgnoreTimeScale ? Time.timeScale : 1;
            this.mTweener.SetDelay(this.Delay);
            this.mTweener.SetEase(this.TimeCurve);
            
            if (this.OnPlayCompleted != null)
                this.mTweener.OnComplete(this.OnPlayCompleted);

            if (this.IsLoop)
            {
                this.mTweener.SetLoops(this.LoopCount, this.LoopType);
            }
        }

        protected virtual void OnCreateTweener()
        {

        }
    }
}

