using System;
using System.Collections.Generic;
using Knight.Core;
using UnityEngine;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public class TextStyleManager : MonoBehaviour
    {
        private static TextStyleManager __instance;
        public  static TextStyleManager Instance { get { return __instance; } }

        public List<TextStyle>          TextStyles;

        private void Awake()
        {
            if (__instance != null)
                __instance = this;
        }
    }
}
