using System;
using System.Collections.Generic;
using Core;
using System.Collections;

namespace WindHotfix.GamePlay
{
    /// <summary>
    /// 游戏子系统，用来驱动游戏组件的业务逻辑
    /// </summary>
    public class GameSystem
    {
        public virtual void ExecuteComponents(List<GameComponent> rComps)
        {
        }

        public virtual IEnumerator ExecuteComponents_Async(List<GameComponent> rComps)
        {
            yield break;
        }

        public virtual void Update(List<GameComponent> rComps)
        {
        }
    }
}