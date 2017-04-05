//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using UnityEngine;
using System.Collections;

namespace Game.Knight
{
    public class GameTest : MonoBehaviour
    {
        public bool             JumpToInit;

        //public ActionManager    ActionMgr;

        void Start()
        {
            CoroutineManager.Instance.Initialize();

            this.StartCoroutine(Start_Async());   
        }

        IEnumerator Start_Async()
        {
            if (JumpToInit)
            {
                var rLevelRequest = Globals.Instance.LoadLevel("Init");
                yield return rLevelRequest;
            }
            else
            {
                //yield return ActionConfig.Instance.Load("actionconfig.ab", "ActionConfig");
                //ActionTemplate rActionTemplate601 = new ActionTemplate()
                //{
                //    ID = 601,
                //    Idle = 601101,
                //    Move = new int[] { 601201 },
                //};
                //if (this.ActionMgr != null)
                //    this.ActionMgr.Initialize(rActionTemplate601);
            }
        }
    }
}