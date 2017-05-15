//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using WindHotfix.Core;

namespace Game.Knight
{
    public class ActionManager : THotfixMB<ActionManager>
    {
        public Animator Animator;
        
        public override void Start()
        {
            this.Animator = this.GameObject.GetComponent<Animator>();
        }
        
        public void PlayMove(bool bIsMove)
        {
            this.Animator.SetBool("IsMove", bIsMove);
        }

        public void PlayRun(bool bIsRun)
        {
            this.Animator.SetBool("IsRun", bIsRun);
        }

        public void PlaySkill(int nAnimID)
        {
            this.Animator.SetInteger("Skill", nAnimID);
        }
    }
}

