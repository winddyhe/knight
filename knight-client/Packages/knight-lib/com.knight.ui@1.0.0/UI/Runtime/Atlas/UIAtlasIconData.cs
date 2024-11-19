using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.UI
{
    [System.Serializable]
    public class UIAtlasIconLink
    {
        public string ABName;
        public List<string> IconNames;
    }

    public class UIAtlasIconData : ScriptableObject
    {
        public List<UIAtlasIconLink> AtlasIconLinks;
    }
}
