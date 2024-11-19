using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.UI
{
    [System.Serializable]
    public class UILinkAtlas
    {
        public string PrefabName;
        public List<string> LinkAtlas;
    }

    public class UIPrefabLinkAtlas : ScriptableObject
    {
        public List<UILinkAtlas> LinkAtlasList;
    }
}
