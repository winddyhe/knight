//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Core;

namespace Framework.WindUI
{
    public class Toast : MonoBehaviour
    {
        private static Toast     __instance;
        public  static Toast     Instance { get { return __instance; } }
                                 
        public Image             TipBG;
        public Text              TipText;
        public CanvasGroup       TipGroup;
                                 
        private CoroutineHandler mCoroutineHandler;

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
                this.gameObject.SetActive(false);
            }
        }

        public void Show(string rTextTip, float rTimeLength = 3.0f)
        {
            if (mCoroutineHandler != null)
            {
                CoroutineManager.Instance.Stop(mCoroutineHandler);
                mCoroutineHandler = null;
            }
            mCoroutineHandler = CoroutineManager.Instance.StartHandler(StartAnim(rTextTip, rTimeLength));
        }

        private IEnumerator StartAnim(string rTextTip, float rTimeLength)
        {
            this.gameObject.SetActive(true);
            this.TipText.text = rTextTip;
            yield return 0;
            this.TipBG.rectTransform.sizeDelta = new Vector2(this.TipText.preferredWidth + 20, this.TipBG.rectTransform.sizeDelta.y);
            float rCurTime = 0.0f;
            yield return new WaitUntil(() =>
            {
                this.TipGroup.alpha = Mathf.Lerp(1, 0, Mathf.InverseLerp(0, rTimeLength, rCurTime));
                rCurTime += Time.deltaTime;
                return rCurTime >= rTimeLength;
            });
            this.TipGroup.alpha = 0;
            this.gameObject.SetActive(false);
        }
    }
}
