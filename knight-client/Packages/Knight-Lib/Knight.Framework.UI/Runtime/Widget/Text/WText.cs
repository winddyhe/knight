//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;

namespace UnityEngine.UI
{
    public class WText : Text
    {
        [SerializeField]
        [HideInInspector]
        public bool mIsUseMultiLang;

        [SerializeField]
        [HideInInspector]
        public int mMultiLangID;

        public int MultiLangID
        {
            get { return mMultiLangID; }
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
            get { return mIsUseMultiLang; }
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
