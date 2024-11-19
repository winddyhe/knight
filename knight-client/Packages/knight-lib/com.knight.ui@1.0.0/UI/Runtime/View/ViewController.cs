using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Framework.UI
{
    public class ViewController
    {
        public View View { get; set; }

        public async UniTask Open()
        {
            await this.OnOpen();
        }

        public void Close()
        {
            this.OnClose();
        }

#pragma warning disable 1998
        protected virtual async UniTask OnOpen()
        {
        }
#pragma warning restore 1998

        protected virtual void OnClose()
        {
        }
    }
}
