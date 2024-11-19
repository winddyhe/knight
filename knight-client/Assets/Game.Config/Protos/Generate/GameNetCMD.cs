using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Proto
{
    public class GAME_CMD
    {
        /// <summary>
        ///登陆命令
        /// </summary>
        public const byte GAME_CMD_LOGIN = 1;
        /// <summary>
        ///主命令
        /// </summary>
        public const byte GAME_CMD_MAIN = 2;
    }

    public class GAME_CMD_LOGIN
    {
        /// <summary>
        ///玩家登陆
        /// </summary>
        public const byte GAME_CMD_LOGIN_PLAYER_CS = 1;
        /// <summary>
        ///玩家登陆返回玩家数据
        /// </summary>
        public const byte GAME_CMD_LOGIN_PLAYER_DATA_S = 2;
    }

    public class GAME_CMD_MAIN
    {
        /// <summary>
        /// 心跳
        /// </summary>
        public const byte GAME_CMD_MAIN_SERVER_TIME_CS = 1;
        /// <summary>
        /// 线上玩家好友数据改变
        /// </summary>
        public const byte GAME_CMD_MAIN_VALUE_UPDATE_S = 2;
    }
}
