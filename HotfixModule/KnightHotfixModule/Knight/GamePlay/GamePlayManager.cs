using System;
using System.Collections.Generic;
using WindHotfix.GamePlay;

namespace Game.Knight
{
    public class GamePlayManager
    {
        public Module BuildLoginModule()
        {
            var rModule = new Module();
            var rLoginEntity = new Entity();

            var rComp = new AssetComponent();
            rComp.AssetName = "KNLogin";
            rComp.AssetPath = "game/ui/knlogin.ab";
            rLoginEntity.AddComponent(rComp);
            rModule.AddEntity(rLoginEntity);

            SubSystem rSystem = new AssetLoadSystem();
            rModule.AddSystem(rSystem);

            return rModule;
        }
    }
}
