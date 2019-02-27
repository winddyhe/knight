using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.Tweening
{
    public enum TweeningActionType
    {
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
        public TweeningActionType ActionType;
    }

    [System.Serializable]
    public class TweeningAction_Position : TweeningAction
    {
        public Vector3  Start;
        public Vector3  End;
    }

    [System.Serializable]
    public class TweeningAction_LocalPosition : TweeningAction
    {
        public Vector3  Start;
        public Vector3  End;
    }

    [System.Serializable]
    public class TweeningAction_Rotate : TweeningAction
    {
        public Vector3  Start;
        public Vector3  End;
    }

    [System.Serializable]
    public class TweeningAction_LocalRotate : TweeningAction
    {
        public Vector3  Start;
        public Vector3  End;
    }

    [System.Serializable]
    public class TweeningAction_LocalScale : TweeningAction
    {
        public Vector3  Start;
        public Vector3  End;
    }

    [System.Serializable]
    public class TweeningAction_Color : TweeningAction
    {
        public Color    Start;
        public Color    End;
    }

    [System.Serializable]
    public class TweeningAction_CanvasAlpha : TweeningAction
    {
        public float    Start;
        public float    End;
    }

    [System.Serializable]
    public class TweeningAction_Delay : TweeningAction
    {
        public float    DelayTime;
    }
    
    public class TweeningAnimator : MonoBehaviour
    {
    }
}
