using UnityEngine;
using Framework.Hotfix;

namespace WindHotfix.Core
{
    public static class GameObjectExpand
    {
        public static T GetHotfixComponent<T>(this GameObject rGameObject) where T : HotfixMB
        {
            string rCompName = typeof(T).FullName;
            var rMBContainers = rGameObject.GetComponents<HotfixMBContainer>();
            for (int i = 0; i < rMBContainers.Length; i++)
            {
                if (rCompName == rMBContainers[i].HotfixName)
                {
                    return rMBContainers[i].MBHotfixObject as T;
                }
            }
            return null;
        }

        public static T AddHotfixComponent<T>(this GameObject rGameObject) where T : HotfixMB
        {
            var rMBContainer = rGameObject.AddComponent<HotfixMBContainer>();
            rMBContainer.HotfixName = typeof(T).FullName;
            rMBContainer.InitHotfixMB();
            return rMBContainer.MBHotfixObject as T;
        }

        public static T ReceiveHotfixComponent<T>(this GameObject rGameObject) where T : HotfixMB
        {
            T rTmpComp = rGameObject.GetHotfixComponent<T>();
            if (rTmpComp == null) rTmpComp = rGameObject.AddHotfixComponent<T>();
            return rTmpComp;
        }
    }
}
