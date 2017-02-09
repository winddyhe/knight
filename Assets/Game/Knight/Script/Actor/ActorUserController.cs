//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using Core;
using Framework;

namespace Game.Knight
{
    public class ActorUserController : MonoBehaviour
    {
        private ActorController mCharacter;
        private Transform       mCameraTrans;

        private Vector3         mCamForward;
        private Vector3         mMove;

        private Vector3         mTempForword = new Vector3(1, 0, 1);

        void Start()
        {
            if (Camera.main != null) 
            {
                mCameraTrans = Camera.main.transform;
            }
            else
            {
                Debug.LogError("Cannot find main camera.");
            }
            mCharacter = this.gameObject.ReceiveComponent<ActorController>();
        }
        
        void FixedUpdate()
        {
            float rHorizontalInput = InputManager.Instance.Horizontal;
            float rVerticalInput   = InputManager.Instance.Vertical;

            if (mCameraTrans != null)
            {
                mCamForward = Vector3.Scale(mCameraTrans.forward, mTempForword).normalized;
                mMove = rVerticalInput * mCamForward + rHorizontalInput * mCameraTrans.right;
            }
            else
            {
                mMove = rVerticalInput * Vector3.forward + rHorizontalInput * Vector3.right;
            }

            bool bIsRun = InputManager.Instance.IsKey(InputKey.Run);

            mCharacter.Move(mMove, bIsRun);
        }
    }
}