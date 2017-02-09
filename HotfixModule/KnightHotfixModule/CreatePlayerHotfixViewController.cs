using Framework.WindUI;
using Game.Knight;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KnightHotfixModule
{
    public class CreatePlayerHotfixViewController : ViewController<CreatePlayerView>
    {
        public CreatePlayerHotfixViewController(CreatePlayerView rView) 
            : base(rView)
        {
        }

        public override void OnOpening()
        {
            Debug.LogError("Hotfix...");
            this.mView.CurrentSelectedItem.StartLoad();
            this.mView.IsOpened = true;
        }

        public override void OnClosing()
        {
            Debug.LogError("Hotfix...");
            this.mView.CurrentSelectedItem.StopLoad();
            this.mView.IsClosed = true;
        }
    }
}
