using Knight.Core;
using Knight.Hotfix.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;

namespace Game
{
    public class ListTestItemDetailView : ViewController
    {
        protected override async Task OnInitialize()
        {
            await base.OnInitialize();
        }

        [DataBinding]
        public void OnBtnBack_Clicked(EventArg rEventArg)
        {
            FrameManager.Instance.CloseView(this.GUID);
        }
    }
}
