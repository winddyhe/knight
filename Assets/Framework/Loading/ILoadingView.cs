//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;

namespace Framework
{
    public interface ILoadingView
    {
        void ShowLoading(string rTextTips);
        void ShowLoading(float rIntervalTime, string rTextTips);
        void SetLoadingProgress(float fProgressValue);
        void HideLoading();
        void SetTips(string rTextTips);
    }
}
