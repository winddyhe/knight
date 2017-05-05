//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections.Generic;
using WindHotfix.Core;

namespace Game.Knight
{
    /// <summary>
    /// 角色账户
    /// </summary>
    public class Account : THotfixSingleton<Account>
    {
        public bool             IsLogin       = false;
        public long             AccountID     = 0;
        public string           AccountName = "";
        public int              ServerID      = 0;

        public NetActor         ActiveActor = null;
        public List<NetActor>   NetActors   = null;
        
        private Account()       { }

        /// <summary>
        /// 创建角色账户
        /// </summary>
        public void Create(long rAccountID, string rAccount, int rServerID)
        {
            this.IsLogin = true;
            this.AccountID = rAccountID;
            this.AccountName = rAccount;
            this.ServerID = rServerID;
        }

        /// <summary>
        /// 新建一个角色
        /// </summary>
        public void CreateActor(string rActorName, int rProfessionalID, long rActorID)
        {
            this.ActiveActor = new NetActor() {
                ActorID = rActorID,
                ActorName = rActorName,
                Level = 1,
                ServerID = this.ServerID
            };
            this.ActiveActor.Professional = GameConfig.Instance.GetActorProfessional(rProfessionalID);
            this.NetActors.Add(this.ActiveActor);
        }
    }
}

