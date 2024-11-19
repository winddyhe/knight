using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.UI
{
    public class RedPoint_ImageNumber : RedPoint
    {
        public int MaxDisplayNumber = 99;

        public GameObject RootGo;
        public Image RedPointImage;
        public TextMeshProUGUI Text;

        protected override void UpdateRedPointNumber(int nRedPointNumber)
        {
            var rRedPointText = string.Empty;
            if (nRedPointNumber == 1)
            {
                rRedPointText = string.Empty;
            }
            else if (nRedPointNumber > 1 && nRedPointNumber <= this.MaxDisplayNumber)
            {
                rRedPointText = nRedPointNumber.ToString();
            }
            else if (nRedPointNumber > this.MaxDisplayNumber)
            {
                rRedPointText = this.MaxDisplayNumber.ToString() + "+";
            }
            if (this.Text)
            {
                this.Text.text = rRedPointText;
            }
            if (this.RootGo)
            {
                this.RootGo.SetActive(nRedPointNumber > 0);
            }
        }
    }
}
