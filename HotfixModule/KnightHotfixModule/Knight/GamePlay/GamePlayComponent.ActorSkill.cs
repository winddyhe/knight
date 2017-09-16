//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using WindHotfix.Core;

namespace Game.Knight
{
    public abstract class GamePlayComponent
    {
        public float                    Start;
        public float                    End;

        protected ActorGamePlayManager  mGamePlayMgr;
        protected List<string>          mArgs;
        protected bool                  mIsStartTriggered = false;
        protected bool                  mIsEndTriggered = false;

        public GamePlayComponent(ActorGamePlayManager rGamePlayMgr, List<string> rArgs)
        {
            this.mGamePlayMgr = rGamePlayMgr;
            this.mArgs = rArgs;
            this.mIsStartTriggered = false;
            this.mIsEndTriggered = false;
        }

        public virtual void Initialize()
        {
            Type rType = this.GetType();
            if (rType == null) return;

            //TODO: ........这里移植过来反射出来的Field的顺序有问题....和以前的不一样
            var rBindingFlags = BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance;
            var rFiledInfos = rType.GetFields(rBindingFlags);

            int i = 0;
            for (i = 0; i < rFiledInfos.Length-2; i++)
            {
                var rFiledType = rFiledInfos[i+2].FieldType;
                var rValue = HotfixReflectAssists.TypeConvert(rFiledType, mArgs[i]);
                rFiledInfos[i+2].SetValue(this, rValue);
            }

            if (rFiledInfos.Length >= 2)
            {
                var rValue0 = HotfixReflectAssists.TypeConvert(rFiledInfos[0].FieldType, mArgs[i++]);
                rFiledInfos[0].SetValue(this, rValue0);

                var rValue1 = HotfixReflectAssists.TypeConvert(rFiledInfos[1].FieldType, mArgs[i++]);
                rFiledInfos[1].SetValue(this, rValue1);
            }

            this.mIsStartTriggered = false;
            this.mIsEndTriggered = false;
        }

        public virtual void Execute(float fCurTime)
        {
            if (fCurTime >= this.Start && !mIsStartTriggered)
            {
                this.OnStart();
                mIsStartTriggered = true;
            }

            if (fCurTime >= this.End && !mIsEndTriggered)
            {
                this.OnEnd();
                mIsEndTriggered = true;
            }

            if (fCurTime >= this.Start && fCurTime <= this.End)
            {
                this.OnUpdate();
            }
        }

        public void Stop()
        {
            this.OnEnd();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnUpdate()
        {
        }
    }

    public class GPCSkill : GamePlayComponent
    {
        public int                      SkillID;

        private List<GamePlayComponent> mComps;

        public GPCSkill(ActorGamePlayManager rGamePlayMgr, List<string> rArgs, List<GamePlayComponent> rComps)
            : base(rGamePlayMgr, rArgs)
        {
            this.mComps = rComps;
        }

        public override void Initialize()
        {
            base.Initialize();

            for (int i = 0; i < this.mComps.Count; i++)
            {
                this.mComps[i].Initialize();
            }

            for (int i = 0; i < this.mComps.Count; i++)
            {
                this.mComps[i].Start += this.Start;
                this.mComps[i].End += this.Start;
            }
        }

        public override void Execute(float fCurTime)
        {
            base.Execute(fCurTime);

            for (int i = 0; i < this.mComps.Count; i++)
            {
                this.mComps[i].Execute(fCurTime);
            }
        }

        protected override void OnEnd()
        {
            for (int i = 0; i < this.mComps.Count; i++)
            {
                this.mComps[i].Stop();
            }
        }
    }

    public class GPCAnimation : GamePlayComponent
    {
        public int              AnimID;

        //private ActionManager   mActionMgr;

        public GPCAnimation(ActorGamePlayManager rGamePlayMgr, List<string> rArgs)
            : base(rGamePlayMgr, rArgs)
        {
        }

        public override void Initialize()
        {
            base.Initialize();
            //mActionMgr = this.mGamePlayMgr.Actor.ActorGo.GetHotfixComponent<ActionManager>();
        }

        protected override void OnStart()
        {
            //mActionMgr.PlaySkill(this.AnimID);
        }

        protected override void OnEnd()
        {
            //mActionMgr.PlaySkill(0);
        }
    }

    public class GPCEffect : GamePlayComponent
    {
        public string   EffectName;
        public string   BoneName;

        public float    OffsetX;
        public float    OffsetY;
        public float    OffsetZ;

        public float    RotateX;
        public float    RotateY;
        public float    RotateZ;

        public float    ScaleX;
        public float    ScaleY;
        public float    ScaleZ;

        public GPCEffect(ActorGamePlayManager rGamePlayMgr, List<string> rArgs)
            : base(rGamePlayMgr, rArgs)
        {
        }

        protected override void OnStart()
        {

        }

        protected override void OnEnd()
        {

        }
    }

    public class GPCAreaCircleHurt : GamePlayComponent
    {
        public int      AreaType;

        public float    Radius;
        public float    TargetOffsetX;
        public float    TargetOffsetY;

        public int      TargetType;

        public GPCAreaCircleHurt(ActorGamePlayManager rGamePlayMgr, List<string> rArgs)
            : base(rGamePlayMgr, rArgs)
        {
        }

        protected override void OnStart()
        {

        }
    }
}


