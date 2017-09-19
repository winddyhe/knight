//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Framework;
using WindHotfix.Core;
using Framework.Hotfix;

namespace Game.Knight
{
    public class GamePadViewJoystick
    {
        public RectTransform        JoystickRootTrans;
        public RectTransform        JoystickFrontTrans;

        public TouchObject          TouchObj1;
        public TouchObject          TouchObj2;

        public float                JoystickMaxDistance = 100;

        private Vector2             JoystickInitPos;
        private HotfixMBContainer   mMBContainer;

        public GamePadViewJoystick(HotfixMBContainer rMBContainer)
        {
            this.mMBContainer       = rMBContainer;
            this.JoystickRootTrans  = rMBContainer.Objects[0].Object as RectTransform;
            this.JoystickFrontTrans = rMBContainer.Objects[1].Object as RectTransform;

            this.JoystickInitPos = this.JoystickRootTrans.anchoredPosition;
            Joystick.Instance.Reset();
        }

        public void Update()
        {
            if (TouchInput.Instance.TouchCount > 0)
            {
                this.TouchObj1 = TouchInput.Instance.GetTouch(0);

                var rTouchPos = this.TouchObj1.position;
                if (rTouchPos.x < Screen.width * 0.3f)
                {
                    if (this.TouchObj1.phase == TouchPhase.Began)
                    {
                        this.JoystickRootTrans.anchoredPosition = this.TouchObj1.position;
                        Joystick.Instance.Reset();
                    }
                    else if (this.TouchObj1.phase == TouchPhase.Moved)
                    {
                        this.JoystickFrontTrans.anchoredPosition += this.TouchObj1.deltaPosition;

                        Vector2 rDir = this.TouchObj1.position - this.JoystickRootTrans.anchoredPosition;
                        if (rDir.sqrMagnitude > this.JoystickMaxDistance * this.JoystickMaxDistance)
                        {
                            this.JoystickFrontTrans.anchoredPosition = rDir.normalized * this.JoystickMaxDistance;
                        }
                        Joystick.Instance.Set(this.JoystickFrontTrans.anchoredPosition.x / this.JoystickMaxDistance,
                                              this.JoystickFrontTrans.anchoredPosition.y / this.JoystickMaxDistance);
                    }
                    else if (this.TouchObj1.phase == TouchPhase.Ended)
                    {
                        this.JoystickRootTrans.anchoredPosition = this.JoystickInitPos;
                        this.JoystickFrontTrans.anchoredPosition = Vector2.zero;
                        Joystick.Instance.Reset();
                    }
                }
            }
            else
            {
                this.JoystickRootTrans.anchoredPosition = this.JoystickInitPos;
                this.JoystickFrontTrans.anchoredPosition = Vector2.zero;
                Joystick.Instance.Reset();
            }
        }
    }
}

