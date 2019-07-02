using Knight.Core;
using Knight.Framework.Hotfix;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Test
{
    public class WindJsonTest : MonoBehaviour
    {
        public TextAsset JsonAsset;

        void Start()
        {
            HotfixManager.Instance.Initialize();
            CoroutineManager.Instance.Initialize();
            EventManager.Instance.Initialize();

            string rDLLPath = "Library/ScriptAssemblies/Game.Hotfix.dll";
            string rPDBPath = "Library/ScriptAssemblies/Game.Hotfix.pdb";
            HotfixManager.Instance.InitApp(rDLLPath, rPDBPath);
            
            HotfixManager.Instance.InvokeStatic("Game.Test.HotfixJsonTest1", "Test", this.JsonAsset.text);
        }
    }
}

