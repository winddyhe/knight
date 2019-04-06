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
        public int                  LoopCount;

        public float                Duration;
        public AnimationCurve       TimeCurve;

        public TweeningActionType   Type;

        public float                StartF = 0.0f;
        public float                EndF = 0.0f;

        public Vector3              StartV3 = Vector3.zero;
        public Vector3              EndV3 = Vector3.zero;

        public Color                StartCol = Color.white;
        public Color                EndCol = Color.white;
        
        public Tweener              Tweener;
    }
    
    public class TweeningAnimator : MonoBehaviour
    {
        public bool                 IsIgnoreTimeScale;
        public bool                 IsUseFixedUpdate;
        public bool                 IsAutoExecute;
        public List<TweeningAction> Actions;

        private void CreateActionTweeners()
        {
            for (int i = 0; i < this.Actions.Count; i++)
            {
                var rActionType = this.Actions[i].Type;
                switch (rActionType)
                {
                    case TweeningActionType.Position:
                        break;
                    case TweeningActionType.LocalPosition:
                        break;
                    case TweeningActionType.Rotate:
                        break;
                    case TweeningActionType.LocalRotate:
                        break;
                    case TweeningActionType.LocalScale:
                        break;
                    case TweeningActionType.Color:
                        break;
                    case TweeningActionType.CanvasAlpha:
                        break;
                    case TweeningActionType.Delay:
                        break;
                }
            }
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
    }
}
