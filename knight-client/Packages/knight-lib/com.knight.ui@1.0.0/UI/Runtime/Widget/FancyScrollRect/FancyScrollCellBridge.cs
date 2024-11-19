using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class FancyScrollCellData
    {
        public ViewModel ViewModel;
    }

    public abstract class FancyScrollCellBridge : MonoBehaviour
    {
        public abstract void Initialize();
        public abstract void UpdateContent(int nIndex, FancyScrollCellData rItemData);
    }
}
