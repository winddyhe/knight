//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Knight.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI
{
    public class WText : Text
    {
        [SerializeField]
        [HideInInspector]
        private bool    mIsUseMultiLang;

        [SerializeField]
        [HideInInspector]
        private int     mMultiLangID;
        
        public  int MultiLangID
        {
            get { return mMultiLangID;  }
            set
            {
                mMultiLangID = value;
                if (this.IsUseMultiLang)
                {
                    this.text = LocalizationManager.Instance.GetMultiLanguage(mMultiLangID);
                }
            }
        }

        public bool IsUseMultiLang
        {
            get { return mIsUseMultiLang;  }
            set { mIsUseMultiLang = value; }
        }

        public override string text
        {
            get
            {
                if (this.mIsUseMultiLang)
                {
                    return LocalizationManager.Instance.GetMultiLanguage(this.mMultiLangID);
                }
                else
                {
                    return base.text;
                }
            }
            set
            {
                base.text = value;
            }
        }
    }
}
