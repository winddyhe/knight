//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections.Generic;
using WindHotfix.Core;

namespace Game.Knight
{
    public class ActorGamePlayManager : THotfixMB<ActorGamePlayManager>
    {
        //public Actor                Actor;
        public List<GPCSkill>       Skills;

        public GPCSkill             CurSkill;

        private bool                mIsPlaying = false;
        private bool                mIsPause   = false;
        private float               mCurTime   = 0.0f;

        //public void Initialize(Actor rActor)
        //{
        //    this.Actor = rActor;

        //    this.BuildSkills(this.Actor.Hero.SkillID);

        //    mIsPlaying = false;
        //    mIsPause = false;
        //    mCurTime = 0.0f;
        //}

        public void Play(int nSkillFixedID)
        {
            if (this.Skills == null) return;
            this.CurSkill = this.GetSkill(nSkillFixedID);

            mIsPlaying = true;
            mIsPause = false;
            mCurTime = 0.0f;
        }

        public override void Update()
        {
            if (!mIsPause) return;
            if (!mIsPlaying) return;

            if (this.CurSkill == null) return;

            this.CurSkill.Execute(this.mCurTime);
        }

        private void BuildSkills(int nActorSkillID)
        {
            var rSymbolObjs = GPCSkillConfig.Instance.GetActorSkill(nActorSkillID);

            this.Skills = new List<GPCSkill>();
            if (rSymbolObjs == null) return;

            for (int i = 0; i < rSymbolObjs.Count; i++)
            {
                List<GamePlayComponent> rComps = new List<GamePlayComponent>();
                for (int j = 0; j < rSymbolObjs[i].Bodies.Count; j++)
                {
                    var rCompType = HotfixReflectAssists.GetType("Game.Knight." + rSymbolObjs[i].Bodies[j].Identifer.Value);
                    var rComp = HotfixReflectAssists.Construct(
                        rCompType,
                        this,
                        rSymbolObjs[i].Bodies[j].ToArgs()) as GamePlayComponent;
                    rComps.Add(rComp);
                }

                GPCSkill rGPCSkill = new GPCSkill(this, rSymbolObjs[i].Head.ToArgs(), rComps);
                this.Skills.Add(rGPCSkill);
            }

            for (int i = 0; i < this.Skills.Count; i++)
            {
                this.Skills[i].Initialize();
            }
        }

        private GPCSkill GetSkill(int nSkillFixedID)
        {
            return this.Skills.Find((rItem) => { return rItem.SkillID == nSkillFixedID; });
        }
    }
}