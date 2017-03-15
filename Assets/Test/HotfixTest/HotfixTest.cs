using UnityEngine;
using System.Collections;
using Core;
using Game.Knight;

namespace Test
{
    public class HotfixTest : MonoBehaviour
    {
        void Awake()
        {
            HotfixManager.Instance.Load();
        }
    }
}