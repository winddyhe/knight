//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Framework.Hotfix;
using WindHotfix.GUI;

namespace Game.Knight
{
    public class GamePadView : THotfixViewController<GamePadView>
    {
        public GamePadViewJoystick Joystick;

        public override void OnInitialize()
        {

        }

        public override void OnOpening()
        {
            this.Joystick = (this.Objects[0].Object as HotfixMBContainer).MBHotfixObject as GamePadViewJoystick;
            this.mIsOpened = true;
        }

        public override void OnClosing()
        {
            this.mIsClosed = true;
        }
    }
}
