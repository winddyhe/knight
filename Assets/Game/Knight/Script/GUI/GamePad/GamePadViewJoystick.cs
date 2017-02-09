//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;
using Framework;

namespace Game.Knight
{
    public class GamePadViewJoystick : MonoBehaviour
    {
#region unity_bind

        public RectTransform    JoystickRootTrans;
        public RectTransform    JoystickFrontTrans;

        public TouchObject      TouchObj1;
        public TouchObject      TouchObj2;

        public float            JoystickMaxDistance = 100;

#endregion

        private Vector2         JoystickInitPos;

        void Start()
        {
            this.JoystickInitPos = this.JoystickRootTrans.anchoredPosition;
            Joystick.Instance.Reset();
        }

        void Update()
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

