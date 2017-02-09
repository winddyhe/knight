//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public enum NetworkMsgCode : int
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown             = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success             = 200,
        /// <summary>
        /// 系统错误
        /// </summary>
        ServerError         = 500,

        /// <summary>
        /// TOKEN不合法
        /// </summary>
        FA_TOKEN_INVALID    = 1001,
        /// <summary>
        /// TOKEN已过期
        /// </summary>
        FA_TOKEN_EXPIRE     = 1002,
        /// <summary>
        /// 登陆账户不存在
        /// </summary>
        FA_USER_NOT_EXIST   = 1003,
        /// <summary>
        /// 登陆密码错误
        /// </summary>
        FA_USER_PASS_ERROR  = 1004,
        /// <summary>
        /// 角色名已存在
        /// </summary>
        FA_ACTOR_IS_EXSIST  = 1005,

        /// <summary>
        /// 没有可用的服务器
        /// </summary>
        FA_NO_SERVER_AVAILABLE = 2001
    }
}