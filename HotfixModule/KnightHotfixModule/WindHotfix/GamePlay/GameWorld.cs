using System;
using System.Collections;
using System.Collections.Generic;

namespace WindHotfix.GamePlay
{
    /// <summary>
    /// 游戏世界对象管理
    ///    1. 创建游戏实体对象和游戏子系统
    ///    2. 驱动游戏游戏子系统完成功能
    /// </summary>
    public class GameWorld
    {
        public List<GameEntity> GameEntities;
        public List<GameSystem> GameSystems;
        
        public virtual IEnumerator Initialize()
        {
            yield break;
        }

        public virtual void Destory()
        {
        }

        public virtual void Update()
        {
        }
    }
}
