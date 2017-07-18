using System;
using System.Collections;
using System.Collections.Generic;
using WindHotfix.GamePlay;

namespace Game.Knight
{
    class GameWorld_CreatePlayer : GameWorld
    {
        public override IEnumerator Initialize()
        {
            this.GameEntities = new List<GameEntity>();
            this.GameSystems = new List<GameSystem>();

            yield break;
        }

        public override void Destory()
        {

        }
    }
}