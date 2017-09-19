//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using WindHotfix.GUI;
using WindHotfix.Core;

namespace Game.Knight
{
    public class GamePadView : TViewController<GamePadView>
    {
        [HotfixBinding("Joystick")]
        public HotfixMBContainer    JoystickMB;

        public GamePadViewJoystick Joystick;

        public override void OnInitialize()
        {
            this.Joystick = new GamePadViewJoystick(this.JoystickMB);
        }

        public override void OnOpening()
        {
            this.mIsOpened = true;
        }

        public override void OnClosing()
        {
            this.mIsClosed = true;
        }

        public override void OnUpdate()
        {
            if (this.Joystick != null)
            {
                this.Joystick.Update();
            }
        }
    }
}
