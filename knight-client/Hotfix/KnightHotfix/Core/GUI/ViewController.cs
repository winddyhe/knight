using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Knight.Hotfix.Core.View;

namespace Knight.Hotfix.Core
{
    public class ViewController : HotfixKnightObject
    {
        public string               GUID        = "";
        public string               ViewName    = "";

        public State                CurState    = State.fixing;

        public bool                 IsOpened;
        public bool                 IsClosed;
        
        public void SetData(string rGUID, string rViewName, State rState)
        {
            this.GUID     = rGUID;
            this.ViewName = rViewName;
            this.CurState = rState;
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
