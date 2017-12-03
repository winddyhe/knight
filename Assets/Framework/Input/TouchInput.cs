//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
//#define INPUT_EDITOR_REMOTE_DEBUG
using UnityEngine;
using System.Collections;

namespace Framework
{
    [System.Serializable]
    public class TouchObject
    {
        ///<summary>
        /// The position delta since last change.
        ///</summary>
        public Vector2      deltaPosition;
        /// <summary>
        /// Amount of time that has passed since the last recorded change in Touch values.
        /// </summary>
        public float        deltaTime;
        /// <summary>
        /// fingerId
        /// </summary>
        public int          fingerId;
        /// <summary>
        /// Describes the phase of the touch.
        /// </summary>
        public TouchPhase   phase;
        /// <summary>
        /// The position of the touch in pixel coordinates.
        /// </summary>
        public Vector2      position;
        /// <summary>
        /// Number of taps.
        /// </summary>
        public int          tapCount;

        public TouchObject(Touch rTouch)
        {
            this.SetTouch(rTouch);
        }

        public void SetTouch(Touch rTouch)
        {
            this.deltaPosition  = rTouch.deltaPosition;
            this.deltaTime      = rTouch.deltaTime;
            this.fingerId       = rTouch.fingerId;
            this.phase          = rTouch.phase;
            this.position       = rTouch.position;
            this.tapCount       = rTouch.tapCount;
        }
    }

    /// <summary>
    /// @TODO: 需要做一个TouchObject的对象池
    /// </summary>
    public class TouchInput : MonoBehaviour
    {
        private static TouchInput   __instance;
        public  static TouchInput   Instance { get { return __instance; } }

        public  int                 touchCount = 0;
        public  MouseTouchMonitor   mouseTouchMonitor;

        void Awake()
        {
            if (__instance == null)
            {
                __instance = this;
            }
        }

        public int TouchCount
        {
            get
            {
#if UNITY_EDITOR && !INPUT_EDITOR_REMOTE_DEBUG
                return this.touchCount;
#else
                this.touchCount = Input.touchCount;
                return Input.touchCount;
#endif
            }
        }
        
        public TouchObject GetTouch(int nTouchIndex)
        {
#if UNITY_EDITOR && !INPUT_EDITOR_REMOTE_DEBUG
            if (nTouchIndex == 0)
                return this.mouseTouchMonitor.TouchObj;
            else
            {
                if (Input.touchCount <= nTouchIndex) return null;
                return new TouchObject(Input.GetTouch(nTouchIndex));
            }
                
#else
            if (Input.touchCount <= nTouchIndex) return null;
            return new TouchObject(Input.GetTouch(nTouchIndex));
#endif
        }

        void Update()
        {
#if UNITY_EDITOR && !INPUT_EDITOR_REMOTE_DEBUG
            if (Input.GetMouseButton(0))
                this.touchCount = 1;
            else
                this.touchCount = 0;
#endif
        }
    }
}
