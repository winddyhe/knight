using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Knight.Hotfix.Core.View;

namespace Knight.Hotfix.Core
{
    public class ViewController : HotfixKnightObject
    {
        public    bool              IsOpened;
        public    bool              IsClosed;

        protected View              mView;

        public void SetView(View rView)
        {
            this.mView = rView;
        }
        
        public void Opening()
        {
            this.IsOpened = true;
            this.OnOpening();
        }

        public void Opened()
        {
            this.OnOpened();
        }

        public void Show()
        {
            this.OnShow();
        }

        public void Hide()
        {
            this.OnHide();
        }

        public void Closing()
        {
            this.IsClosed = true;
            this.OnClosing();
        }

        public void Closed()
        {
            this.OnClosed();
        }
        
        #region Virtual Function
        protected override Task OnInitialize()
        {
            return base.OnInitialize();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDispose()
        {
        }

        protected virtual void OnOpening()
        {
        }

        protected virtual void OnOpened()
        {
        }

        protected virtual void OnShow()
        {
        }

        protected virtual void OnHide()
        {
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void OnClosed()
        {
        }
        #endregion
    }
}
