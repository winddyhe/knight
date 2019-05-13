using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;

namespace Knight.Framework.Tweening {
    public class TweeningAnimationFactory
    {
        public static void CreateTweenBehaviour(TweeningAction rTweeningAction , GameObject rGameObject) {
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
            }
        }

        /// <summary>
        /// Postion
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetPostion_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject) {
            var tras = rGameObject.GetComponent<Transform>();
            tras.position = rTweeningAction.StartV3;
            rTweeningAction.Tweener = DOTween.To(() => tras.position, (x) => { tras.position = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
        }

        /// <summary>
        /// LocalPostion
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalPostion_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            tras.localPosition = rTweeningAction.StartV3;
            rTweeningAction.Tweener = DOTween.To(() => tras.localPosition, (x) => { tras.localPosition = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
        }

        /// <summary>
        /// Rotate
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetRotate_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            tras.rotation = Quaternion.Euler(rTweeningAction.StartV3);
            TweenerCore<Quaternion, Vector3, QuaternionOptions> t = DOTween.To(() => tras.rotation, (x) => { tras.rotation = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            t.plugOptions.rotateMode = RotateMode.FastBeyond360;
            rTweeningAction.Tweener = t;
        }

        /// <summary>
        /// LocalRotate
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalRotate_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject) {
            var tras = rGameObject.GetComponent<Transform>();
            tras.localRotation = Quaternion.Euler(rTweeningAction.StartV3);
            TweenerCore <Quaternion, Vector3, QuaternionOptions> t = DOTween.To(() => tras.localRotation, (x) => { tras.localRotation = x; }, rTweeningAction.EndV3, rTweeningAction.Duration);
            t.plugOptions.rotateMode = RotateMode.FastBeyond360;
            rTweeningAction.Tweener = t;
        }

        /// <summary>
        /// LocalScale
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetLocalScale_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject)
        {
            var tras = rGameObject.GetComponent<Transform>();
            rGameObject.transform.localScale = rTweeningAction.StartV3;
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
            if (meshRenderers != null && meshRenderers.Length > 0) {
                foreach (var item in meshRenderers)
                {
                    item.material.color = rTweeningAction.StartCol;
                }
                rTweeningAction.Tweener = DOTween.To( 
                    () =>  meshRenderers[0].material.color ,
                    (x) => {
                        foreach (var item in meshRenderers)
                        {
                            item.material.color = x;
                        }
                    },
                    rTweeningAction.EndCol,
                    rTweeningAction.Duration
                    );
            }

            var graphics = rGameObject.GetComponents<Graphic>();
            if (graphics != null && graphics.Length > 0) {
                foreach (var item in graphics)
                {
                    item.color = rTweeningAction.StartCol;
                }
                rTweeningAction.Tweener = DOTween.To(
                    () => graphics[0].color,
                    (x) => {
                        foreach (var item in graphics)
                        {
                            item.color = x;
                        }
                    },
                    rTweeningAction.EndCol,
                    rTweeningAction.Duration
                    );
            }
        }

        /// <summary>
        /// CanvasAlpha
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetCanvasAlpha_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject) {
            var canvasAlpha = rGameObject.GetComponent<CanvasGroup>();
            if (canvasAlpha != null) {
                canvasAlpha.alpha = rTweeningAction.StartF;
                rTweeningAction.Tweener = DOTween.To(() => canvasAlpha.alpha, (x) => canvasAlpha.alpha = x, rTweeningAction.EndF, rTweeningAction.Duration);
            }
        }

        /// <summary>
        /// Delay
        /// </summary>
        /// <param name="rTweeningAction"></param>
        /// <param name="rGameObject"></param>
        public static void SetDelay_Behaviour(TweeningAction rTweeningAction, GameObject rGameObject) {
            rTweeningAction.Tweener = DOTween.To(()=>{ return 0;} , (x) => { } , 0 , rTweeningAction.Duration);
        }
    }
}

