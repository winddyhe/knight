//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using Knight.Core;
using System;

namespace Knight.Framework.Input
{
    /// <summary>
    /// 键盘输入
    /// </summary>
    public class KeyboardInput : BaseInput
    {
        public Dict<InputKey, KeyCode> mKeys = new Dict<InputKey, KeyCode>()
        {
            { InputKey.Jump,   KeyCode.Space     },
            { InputKey.Attack, KeyCode.C         },
            { InputKey.Run,    KeyCode.LeftShift },
            { InputKey.Skill1, KeyCode.Keypad1   },
            { InputKey.Skill2, KeyCode.Keypad2   },
            { InputKey.Skill3, KeyCode.Keypad3   },
            { InputKey.Skill4, KeyCode.Keypad4   },
            { InputKey.Skill5, KeyCode.R         },
            { InputKey.Skill6, KeyCode.F         }
        };

        public KeyboardInput()
            : base()
        {
        }

        public override float Horizontal
        {
            get { return UnityEngine.Input.GetAxis("Horizontal"); }
        }
        
        public override float Vertical
        {
            get { return UnityEngine.Input.GetAxis("Vertical"); }
        }

        public override bool IsKeyDown(InputKey rInputKey)
        {
            KeyCode rKeyCode = KeyCode.None;
            if (mKeys.TryGetValue(rInputKey, out rKeyCode))
            {
                return UnityEngine.Input.GetKeyDown(rKeyCode);
            }
            return false;
        }

        public override bool IsKeyUp(InputKey rInputKey)
        {
            KeyCode rKeyCode = KeyCode.None;
            if (mKeys.TryGetValue(rInputKey, out rKeyCode))
            {
                return UnityEngine.Input.GetKeyUp(rKeyCode);
            }
            return false;
        }

        public override bool IsKey(InputKey rInputKey)
        {
            KeyCode rKeyCode = KeyCode.None;
            if (mKeys.TryGetValue(rInputKey, out rKeyCode))
            {
                return UnityEngine.Input.GetKey(rKeyCode);
            }
            return false;
        }
    }
}
