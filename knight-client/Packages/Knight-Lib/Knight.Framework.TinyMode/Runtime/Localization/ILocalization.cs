//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;

namespace Knight.Framework.TinyMode
{
    public interface ILocalization
    {
        string GetMultiLanguage(int nMultiLangID);
    }

    public class LocalizationManager
    {
        private ILocalization mLocal;

        static LocalizationManager GInstance = null;
        public static LocalizationManager Instance
        {
            get
            {
                if (null == GInstance)
                    GInstance = new LocalizationManager();
                return GInstance;
            }
        }

        private LocalizationManager()
        {
        }

        public void Initialize(ILocalization rLocal)
        {
            this.mLocal = rLocal;
        }

        public string GetMultiLanguage(int nMultiLangID)
        {
            if (this.mLocal == null) return nMultiLangID.ToString();
            return this.mLocal.GetMultiLanguage(nMultiLangID);
        }
    }
}
