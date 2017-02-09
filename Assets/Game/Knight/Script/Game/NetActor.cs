//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Game.Knight
{
    /// <summary>
    /// 网络角色
    /// </summary>
    public class NetActor
    {
        /// <summary>
        /// 角色的网络ID
        /// </summary>
        public long     ActorID;
        /// <summary>
        /// 角色的网络名称
        /// </summary>
        public string   ActorName;
        /// <summary>
        /// 等级
        /// </summary>
        public int      Level;
        /// <summary>
        /// 角色所在服务器的ID
        /// </summary>
        public int      ServerID;
        /// <summary>
        /// 角色的职业类型
        /// </summary>
        public ActorProfessional Professional;
    }
}


