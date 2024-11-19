using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Knight.Framework.Hotfix
{
    public interface IHotfixMainLogic
    {
        UniTask Initialize();
        void Update(float fDeltaTime);
        void LateUpdate(float fDeltaTime);
        void Destroy();
    }

    public class HotfixMainLogicManager
    {
        public static IHotfixMainLogic Instance;
    }
}
