//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Framework.WindUI;
using Framework;

namespace Game.Knight
{
    public class GamePadView : View
    {
        public GamePadViewJoystick Joystick;

        protected override void InitializeViewController()
        {
            //this.viewController = new GamePadViewController(this);
        }
    }

    public class GamePadViewController : ViewController
    {
        public GamePadViewController(View rView) 
            : base()
        {
        }

        public override void OnOpening()
        {
            //this.mView.IsOpened = true;
        }

        public override void OnClosing()
        {
            //this.mView.IsClosed = true;
        }
    }
}
