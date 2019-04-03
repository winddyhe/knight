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
    public class TweeningActionBase
    {
    }

    [System.Serializable]
    public class TweeningAction_Position : TweeningActionBase
    {
        public Vector3              Start;
        public Vector3              End;
    }

    [System.Serializable]
    public class TweeningAction_LocalPosition : TweeningActionBase
    {
        public Vector3              Start;
        public Vector3              End;
    }

    [System.Serializable]
    public class TweeningAction_Rotate : TweeningActionBase
    {
        public Vector3              Start;
        public Vector3              End;
    }

    [System.Serializable]
    public class TweeningAction_LocalRotate : TweeningActionBase
    {
        public Vector3              Start;
        public Vector3              End;
    }

    [System.Serializable]
    public class TweeningAction_LocalScale : TweeningActionBase
    {
        public Vector3              Start;
        public Vector3              End;
    }

    [System.Serializable]
    public class TweeningAction_Color : TweeningActionBase
    {
        public Color                Start;
        public Color                End;
    }

    [System.Serializable]
    public class TweeningAction_CanvasAlpha : TweeningActionBase
    {
        public float                Start;
        public float                End;
    }

    [System.Serializable]
    public class TweeningAction_Delay : TweeningActionBase
    {
    }

    [System.Serializable]
    public class TweeningAction
    {
        public bool                 IsEnable;
        public bool                 IsFold;

        public bool                 IsLoop;
        public LoopType             LoopType;
        public int                  LoopCount;

        public float                Duration;
        public AnimationCurve       TimeCurve;

        public TweeningActionType   Type;
        public TweeningActionBase   Action;

        [NonSerialized]
        public Tweener              Tweener;
    }
    
    public class TweeningAnimator : MonoBehaviour
    {
        public List<TweeningAction> Actions;
    }
}
