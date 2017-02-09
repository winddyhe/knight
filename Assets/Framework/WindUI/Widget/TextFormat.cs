//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Core;

namespace Framework.WindUI
{
    [ExecuteInEditMode, RequireComponent(typeof(Text))]
    public class TextFormat : MonoBehaviour
    {
        public Text     Text;
        public string   FormatText;

        void Awake()
        {
            this.Text = this.gameObject.ReceiveComponent<Text>();
        }
        
        public void Set(params object[] args)
        {
            if (this.Text == null)
                this.Text = this.gameObject.ReceiveComponent<Text>();
            this.Text.text = string.Format(this.FormatText, args);
        }

        [ContextMenu("Excute")]
        private void Excute()
        {
            this.Text.text = this.FormatText;
        }
    }
}



