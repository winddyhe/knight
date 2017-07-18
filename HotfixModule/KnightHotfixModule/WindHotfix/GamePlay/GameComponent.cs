using System;
using System.Collections.Generic;

namespace WindHotfix.GamePlay
{
    /// <summary>
    /// 游戏实体的组件，只包含数据，不包含操作
    /// </summary>
    public class GameComponent
    {
        public string       ID;
        public GameEntity   ParentEntiy;
    }
}
