//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Game.Knight
{
    public class ActionManager : MonoBehaviour
    {
        public Animator Animator;
        
        void Start()
        {
            this.Animator = this.GetComponent<Animator>();
        }
        
        void Update()
        {
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

