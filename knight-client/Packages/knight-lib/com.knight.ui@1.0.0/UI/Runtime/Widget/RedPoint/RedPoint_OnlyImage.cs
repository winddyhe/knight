using UnityEngine;
using UnityEngine.UIElements;

namespace Knight.Framework.UI
{
    public class RedPoint_OnlyImage : RedPoint
    {
        public GameObject RootGo;
        public Image RedPointImage;

        protected override void UpdateRedPointNumber(int nRedPointNumber)
        {
            if (this.RootGo)
            {
                this.RootGo.SetActive(nRedPointNumber > 0);
            } 
        }
    }
}
