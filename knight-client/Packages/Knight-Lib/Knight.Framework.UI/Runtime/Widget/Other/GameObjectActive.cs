using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    public class GameObjectActive : MonoBehaviour
    {
        [SerializeField][ReadOnly]
        private bool    mIsActive;

        public  bool    IsActive
        {
            get
            {
                mIsActive = this.gameObject.activeSelf;
                return mIsActive;
            }
            set
            {
                mIsActive = value;
                this.gameObject.SetActive(mIsActive);
            }
        }

        public bool     IsDeActive
        {
            get
            {
                mIsActive = this.gameObject.activeSelf;
                return !mIsActive;
            }
            set
            {
                mIsActive = !value;
                this.gameObject.SetActive(mIsActive);
            }
        }
    }
}
