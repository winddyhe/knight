using Knight.Core;
using Knight.Framework;
using Knight.Framework.Hotfix;
using Knight.Framework.TypeResolve;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Test
{
    public class DataBindingTest3 : MonoBehaviour
    {
        void Awake()
        {
            HotfixManager.Instance.Initialize();
            CoroutineManager.Instance.Initialize();
            EventManager.Instance.Initialize();

            TypeResolveManager.Instance.AddAssembly("Game");
            TypeResolveManager.Instance.AddAssembly("Tests");
            TypeResolveManager.Instance.AddAssembly("KnightHotfix", true);
        }
    }
}


