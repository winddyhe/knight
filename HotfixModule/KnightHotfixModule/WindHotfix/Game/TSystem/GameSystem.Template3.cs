//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace WindHotfix.Game
{
    public class TGameSystem<T1, T2, T3> : GameSystem
        where T1 : GameComponent
        where T2 : GameComponent
        where T3 : GameComponent
    {
        public override void Start()
        {
            this.IsActive = true;
            this.mResultComps = new GCContainer();

            ECSManager.Instance.ForeachEntities(this.mResultComps, (rComps) =>
            {
                this.OnStart(this.mResultComps.Get<T1>(), this.mResultComps.Get<T2>(), this.mResultComps.Get<T3>());
            }, typeof(T1), typeof(T2), typeof(T3));
        }

        public override void Update()
        {
            if (!this.IsActive) return;

            ECSManager.Instance.ForeachEntities(this.mResultComps, (rComps) =>
            {
                this.OnUpdate(this.mResultComps.Get<T1>(), this.mResultComps.Get<T2>(), this.mResultComps.Get<T3>());
            }, typeof(T1), typeof(T2), typeof(T3));
        }

        public override void Stop()
        {
            this.IsActive = false;

            ECSManager.Instance.ForeachEntities(this.mResultComps, (rComps) =>
            {
                this.OnStop(this.mResultComps.Get<T1>(), this.mResultComps.Get<T2>(), this.mResultComps.Get<T3>());
            }, typeof(T1), typeof(T2), typeof(T3));
        }

        protected virtual void OnStart(T1 rComp1, T2 rComp2, T3 rComp3)
        {
        }

        protected virtual void OnUpdate(T1 rComp1, T2 rComp2, T3 rComp3)
        {
        }

        protected virtual void OnStop(T1 rComp1, T2 rComp2, T3 rComp3)
        {
        }
    }
}