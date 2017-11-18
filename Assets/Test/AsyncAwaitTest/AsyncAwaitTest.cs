//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;

namespace Test
{
    public class AsyncAwaitTest : MonoBehaviour
    {
        private bool                        mIsComp     = false;
        private float                       mCurTime    = 0.0f;
        private TaskCompletionSource<bool>  mTCS;

        async void Start()
        {
            Debug.Log("Waiting 1 Sencond...");

            await Task.Delay(TimeSpan.FromSeconds(1));
            Debug.Log("Done...");

            Start_Async_1();
        }

        private async void Start_Async_1()
        {
            mIsComp = false;
            Debug.LogError("1111");

            await Start_Async1();

            Debug.LogError("2222");
        }

        private Task<bool> Start_Async1()
        {
            this.mTCS = new TaskCompletionSource<bool>();
            return this.mTCS.Task;
        }

        void Update()
        {
            if (mCurTime > 5 && !mIsComp)
            {
                mIsComp = true;
                this.mTCS.SetResult(true);
            }
            mCurTime += Time.deltaTime;
        }
    }
}


