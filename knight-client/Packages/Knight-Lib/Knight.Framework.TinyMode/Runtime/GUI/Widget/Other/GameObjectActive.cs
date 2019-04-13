//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace Knight.Framework.TinyMode.UI
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
