//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Knight
{
    public partial class Actor
    {
        /// <summary>
        /// Actor的NetActor
        /// </summary>
        public NetActor         NetActor;
        /// <summary>
        /// Actor的Hero信息
        /// </summary>
        public Hero             Hero;
        /// <summary>
        /// Actor的Avatar信息
        /// </summary>
        public Avatar           Avatar;
        /// <summary>
        /// 展示用的Actor
        /// </summary>
        public ExhibitActor     ExhibitActor;
        /// <summary>
        /// 角色的实例GameObject对象
        /// </summary>
        public GameObject       ActorGo
        {
            get {
                if (this.ExhibitActor == null) return null;
                return this.ExhibitActor.ActorGo;
            }
        }
    }

    /// <summary>
    /// 展示用的Actor
    /// </summary>
    public class ExhibitActor
    {
        /// <summary>
        /// Actor的Avatar信息
        /// </summary>
        public Actor            Actor;
        /// <summary>
        /// Actor的GameObject对象
        /// </summary>
        public GameObject       ActorGo; 
    }
}
