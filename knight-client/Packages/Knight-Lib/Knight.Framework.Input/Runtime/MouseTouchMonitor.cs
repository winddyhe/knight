//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Knight.Framework.Input
{
    public class MouseTouchMonitor : MonoBehaviour
    {
        public TouchObject TouchObj;

        private float      mCurTime;
        private float      mLastTime;

        private Vector2    mCurPos;
        private Vector2    mLastPos;

        private bool       mIsTouched = false;

        public  bool       IsTouched { get { return mIsTouched; } }
        
        void Update()
        {
            if (UnityEngine.Input.GetMouseButton(0))
            {
                if (this.TouchObj.deltaPosition == Vector2.zero)
                    this.TouchObj.phase = TouchPhase.Stationary;
                else
                    this.TouchObj.phase = TouchPhase.Moved;

                this.TouchObj.deltaTime = this.mCurTime - this.mLastTime;
                this.TouchObj.deltaPosition = this.mCurPos - this.mLastPos;

                mLastTime = mCurTime;
                mCurTime += Time.deltaTime;

                mLastPos = mCurPos;
                mCurPos = UnityEngine.Input.mousePosition;
                this.TouchObj.position = mCurPos;

                this.mIsTouched = true;
            }
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                mCurTime = mLastTime = 0;
                mCurPos = mLastPos = UnityEngine.Input.mousePosition;

                this.TouchObj.fingerId = -1000;
                this.TouchObj.position = UnityEngine.Input.mousePosition;
                this.TouchObj.phase = TouchPhase.Began;
                this.mIsTouched = true;
            }
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                this.TouchObj.fingerId = -1000;
                this.TouchObj.position = UnityEngine.Input.mousePosition;
                this.TouchObj.phase = TouchPhase.Ended;
                this.mIsTouched = false;
            }
        }
    }
}

