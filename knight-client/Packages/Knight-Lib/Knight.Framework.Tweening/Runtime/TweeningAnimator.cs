using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

namespace Knight.Framework.Tweening
{
    public enum TweeningActionType
    {
        None,
        Position,
        LocalPosition,
        Rotate,
        LocalRotate,
        LocalScale,
        Color,
        CanvasAlpha,
        Delay,
    }

    [System.Serializable]
    public class TweeningAction
    {
        public bool                 IsEnable = false;
        public bool                 IsFold = false;

        public bool                 IsLoop = false;
        public LoopType             LoopType;
        public int                  LoopCount = -1;

        public float                Duration = 0.35f;
        public AnimationCurve       TimeCurve = AnimationCurve.EaseInOut(0 , 0 , 1 , 1);

        public TweeningActionType   Type;

        public float                StartF = 0.0f;
        public float                EndF = 0.0f;

        public Vector3              StartV3 = Vector3.zero;
        public Vector3              EndV3 = Vector3.zero;

        public Color                StartCol = Color.white;
        public Color                EndCol = Color.white;
        
        public Tweener              Tweener;
    }
    
    [AddComponentMenu("UI Animation/TweeningAnimator")]
    public class TweeningAnimator : MonoBehaviour
    {
        public bool                 IsIgnoreTimeScale;
        public bool                 IsUseFixedUpdate;
        public bool                 IsAutoExecute = true;
        public bool                 IsLoopAnimator;

        public List<TweeningAction> Actions;

        private int                 mAnimationCount;
        
        private void CreateActionTweeners()
        {
            mAnimationCount = 0;
            for (int i = 0; i < this.Actions.Count; i++)
            {
                TweeningAnimationFactory.CreateTweenBehaviour(Actions[i] , this.gameObject);
                this.SetUpTweener(Actions[i]);
                if (this.Actions[i].Tweener != null)
                {
                    this.Actions[i].Tweener.onComplete = this.nextAnimation;
                }
            }
            if (IsAutoExecute)
            {
                 this.Play();
            }
        }

        public void Play()
        {
            if (this.Actions == null || this.Actions.Count == 0) return;
            this.Actions[mAnimationCount].Tweener.Play();
        }

        public void Stop()
        {
            if (this.Actions == null) return;
            for (int i = 0; i < this.Actions.Count; i++)
            {
                if (this.Actions[i].Tweener == null) return;
                this.Actions[i].Tweener.Kill(false);
            }
            this.mAnimationCount = 0;
        }

        public void Pause()
        {
            if (this.Actions == null) return;
            this.Actions[mAnimationCount].Tweener.Pause();
        }

        public Tweener GetPlayingTweener()
        {
            if (this.Actions == null || this.Actions.Count == 0)
                return null;
            return this.Actions[mAnimationCount].Tweener;
        }

        private void SetUpTweener(TweeningAction rTweenAction)
        {
            if (rTweenAction.Tweener == null) return;

            // 先暂停
            rTweenAction.Tweener.Pause();
            rTweenAction.Tweener.SetUpdate(this.IsUseFixedUpdate ? UpdateType.Fixed : UpdateType.Normal, false);
            rTweenAction.Tweener.timeScale = this.IsIgnoreTimeScale ? Time.timeScale : 1;
            rTweenAction.Tweener.SetEase(rTweenAction.TimeCurve);
            // 是否循环
            if (rTweenAction.IsLoop)
            {
                rTweenAction.Tweener.SetLoops(rTweenAction.LoopCount, rTweenAction.LoopType);
            }
        }

        private void nextAnimation()
        {

            this.mAnimationCount++;
            if (this.mAnimationCount == this.Actions.Count)
            {
                if (this.IsLoopAnimator)
                    this.CreateActionTweeners();
            }
            else if (this.Actions[mAnimationCount].Tweener != null)
                this.Actions[mAnimationCount].Tweener.Play();
            else
                this.nextAnimation();
        }

        private void OnEnable()
        {
            this.CreateActionTweeners();
        }

        private void OnDisable()
        {
            this.Stop();    
        }
    }
}
