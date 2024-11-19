using Cysharp.Threading.Tasks;
using Knight.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class ViewAssetConfig
    {
        protected Dictionary<string, ViewModuleConfig> mViewModuleConfigs;
        
        public Dictionary<string, ViewModuleConfig> ViewModuleConfigs => this.mViewModuleConfigs;

        public void Initialize()
        {
            this.OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }
    }
}
