using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;
using System;

namespace Knight.Framework.Tweening {
    public class TweeningAnimationFactory
    {
        public static void CreateTweenBehaviour(TweeningAction rTweeningAction , GameObject rGameObject)
        {
            var rActionType = rTweeningAction.Type;
            switch (rActionType)
            {
                case TweeningActionType.Position:
                    SetPostion_Behaviour(rTweeningAction , rGameObject);
                    break;
                case TweeningActionType.LocalPosition:
                    SetLocalPostion_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.Rotate:
                    SetRotate_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.LocalRotate:
                    SetLocalRotate_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.LocalScale:
                    SetLocalScale_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.Color:
                    SetColor_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.CanvasAlpha:
                    SetCanvasAlpha_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.Delay:
                    SetDelay_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.ImageAmount:
                    SetImageAmount_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.RectTransPos:
                    SetRectTransPos_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.RectTransPosX:
                    SetRectTransPosX_Behaviour(rTweeningAction, rGameObject);
                    break;
                case TweeningActionType.RectTransPosY:
                    SetRectTransPosY_Behaviour(rTweeningAction, rGameObject);
                    break;
            }
        }

        /// <summary>
        /// Postion
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetPostion_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            rTweeningAction.Tweener = DOTween.To(() => tras.position, (x) => { tras.position = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            rTweeningAction.SetPlayStartValue = () => { tras.position = rTweeningAction.StartV3; };
            rTweeningAction.SetDefultValue = () => { tras.position = rTweeningAction.DefultVector3; };
        }

        /// <summary>
        /// LocalPostion
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalPostion_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            rTweeningAction.Tweener = DOTween.To(() => tras.localPosition, (x) => { tras.localPosition = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            rTweeningAction.SetPlayStartValue = () => { tras.localPosition = rTweeningAction.StartV3; };
            rTweeningAction.SetDefultValue = () => { tras.localPosition = rTweeningAction.DefultVector3; };
        }

        /// <summary>
        /// Rotate
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetRotate_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            TweenerCore<Quaternion, Vector3, QuaternionOptions> t = DOTween.To(() => tras.rotation, (x) => { tras.rotation = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            t.plugOptions.rotateMode = RotateMode.FastBeyond360;
            rTweeningAction.Tweener = t;
            rTweeningAction.SetPlayStartValue = () => { tras.eulerAngles = rTweeningAction.StartV3; };
            rTweeningAction.SetDefultValue = () => { tras.localEulerAngles = rTweeningAction.DefultVector3; };
        }

        /// <summary>
        /// LocalRotate
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalRotate_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            TweenerCore <Quaternion, Vector3, QuaternionOptions> t = DOTween.To(() => tras.localRotation, (x) => { tras.localRotation = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            t.plugOptions.rotateMode = RotateMode.FastBeyond360;
            rTweeningAction.Tweener = t;
            rTweeningAction.SetPlayStartValue = () => { tras.localEulerAngles = rTweeningAction.StartV3; };
            rTweeningAction.SetDefultValue = () => { tras.eulerAngles = rTweeningAction.DefultVector3; };
        }

        /// <summary>
        /// LocalScale
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalScale_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            rTweeningAction.SetDefultValue = () => { tras.localScale = rTweeningAction.DefultVector3;};
            rTweeningAction.SetPlayStartValue = () => { tras.localScale = rTweeningAction.StartV3; };
            rTweeningAction.Tweener = DOTween.To(()=> tras.localScale , (x)=> { tras.localScale = x; } , rTweeningAction.EndV3 , rTweeningAction.Duration);
        }

        /// <summary>
        /// Color
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetColor_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var meshRenderers = rGameObject.GetComponents<MeshRenderer>();
            if (meshRenderers != null && meshRenderers.Length > 0)
            {
                rTweeningAction.Tweener = DOTween.To( 
                    () =>  meshRenderers[0].material.color ,
                    (x) => 
                    {
                        foreach (var item in meshRenderers)
                        {
                            item.material.color = x;
                        }
                    },
                    rTweeningAction.EndCol,
                    rTweeningAction.Duration
                    );
                rTweeningAction.SetPlayStartValue = () =>
                {
                    for (int i = 0; i < meshRenderers.Length; i++)
                    {
                        meshRenderers[i].material.color = rTweeningAction.StartCol;
                    }
                };
                rTweeningAction.SetDefultValue = () =>
                {
                    for (int i = 0; i < meshRenderers.Length; i++)
                    {
                        meshRenderers[i].material.color = rTweeningAction.DefultColor;
                    }
                };
            }

            var graphics = rGameObject.GetComponents<Graphic>();
            if (graphics != null && graphics.Length > 0)
            {
                rTweeningAction.Tweener = DOTween.To(
                    () => graphics[0].color,
                    (x) => 
                    {
                        foreach (var item in graphics)
                        {
                            item.color = x;
                        }
                    },
                    rTweeningAction.EndCol,
                    rTweeningAction.Duration
                    );
                
                rTweeningAction.SetPlayStartValue = () =>
                {
                    for (int i = 0; i < graphics.Length; i++)
                    {
                        graphics[i].color = rTweeningAction.StartCol;
                    }
                };
                
                rTweeningAction.SetDefultValue = () =>
                {
                    for (int i = 0; i < graphics.Length; i++)
                    {
                        graphics[i].color = rTweeningAction.DefultColor;
                    }
                };
            }
        }

        /// <summary>
        /// CanvasAlpha
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetCanvasAlpha_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject) {
            var canvasAlpha = rGameObject.GetComponent<CanvasGroup>();
            if (canvasAlpha != null)
            {
                rTweeningAction.SetDefultValue = () => { canvasAlpha.alpha = rTweeningAction.DefultFloat; };
                rTweeningAction.SetPlayStartValue = () => { canvasAlpha.alpha = rTweeningAction.StartF; };
                rTweeningAction.Tweener = DOTween.To(
                    () => canvasAlpha.alpha, 
                    (x) => canvasAlpha.alpha = x, 
                    rTweeningAction.EndF, 
                    rTweeningAction.Duration
                    );
            }
        }

        /// <summary>
        /// Delay
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetDelay_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            rTweeningAction.Tweener = DOTween.To(
                ()=>{ return 0;} , 
                (x) => { } ,
                0 ,
                rTweeningAction.Duration
                );
        }

        public static void SetImageAmount_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var rImage = rGameObject.GetComponent<Image>();
            if (rImage != null)
            {
                rTweeningAction.SetDefultValue = () => { rImage.fillAmount = rTweeningAction.DefultFloat; };
                rTweeningAction.SetPlayStartValue = () => { rImage.fillAmount = rTweeningAction.StartF; };
                rTweeningAction.Tweener = DOTween.To(
                    () => { return rImage.fillAmount; },
                    (x) => { rImage.fillAmount = x; },
                    rTweeningAction.EndF,
                    rTweeningAction.Duration);
            }
        }

        private static void SetRectTransPos_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var rRectTrans = rGameObject.GetComponent<RectTransform>();
            if (rRectTrans != null)
            {
                rTweeningAction.SetDefultValue = () => { rRectTrans.anchoredPosition = rTweeningAction.DefultVector3; };
                rTweeningAction.SetPlayStartValue = () => { rRectTrans.anchoredPosition = rTweeningAction.StartV3; };
                rTweeningAction.Tweener = DOTween.To(
                    () => { return new Vector2(rRectTrans.anchoredPosition.x, rRectTrans.anchoredPosition.y); },
                    (x)=> { rRectTrans.anchoredPosition = x; },
                    new Vector2(rTweeningAction.EndV3.x, rTweeningAction.EndV3.y),
                    rTweeningAction.Duration);
            }
        }

        private static void SetRectTransPosX_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var rRectTrans = rGameObject.GetComponent<RectTransform>();
            if (rRectTrans != null)
            {
                rTweeningAction.SetDefultValue = () => { rRectTrans.anchoredPosition = new Vector2(rTweeningAction.DefultFloat, rRectTrans.anchoredPosition.y); };
                rTweeningAction.SetPlayStartValue = () => { rRectTrans.anchoredPosition = new Vector2(rTweeningAction.StartF, rRectTrans.anchoredPosition.y); };
                rTweeningAction.Tweener = DOTween.To(
                    () => { return rRectTrans.anchoredPosition.x; },
                    (x) => { rRectTrans.anchoredPosition = new Vector2(x, rRectTrans.anchoredPosition.y); },
                    rTweeningAction.EndF,
                    rTweeningAction.Duration);
            }
        }

        private static void SetRectTransPosY_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var rRectTrans = rGameObject.GetComponent<RectTransform>();
            if (rRectTrans != null)
            {
                rTweeningAction.SetDefultValue = () => { rRectTrans.anchoredPosition = new Vector2(rRectTrans.anchoredPosition.x, rTweeningAction.DefultFloat); };
                rTweeningAction.SetPlayStartValue = () => { rRectTrans.anchoredPosition = new Vector2(rRectTrans.anchoredPosition.x, rTweeningAction.StartF); };
                rTweeningAction.Tweener = DOTween.To(
                    () => { return rRectTrans.anchoredPosition.y; },
                    (y) => { rRectTrans.anchoredPosition = new Vector2(rRectTrans.anchoredPosition.x, y); },
                    rTweeningAction.EndF,
                    rTweeningAction.Duration);
            }
        }
    }
}

