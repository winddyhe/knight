using System;
using System.Collections.Generic;
using Knight.Core;
using Knight.Framework.Hotfix;
using UnityEngine.UI;

namespace Knight.Hotfix.Core
{
    public class ViewModelManager : THotfixSingleton<ViewModelManager>
    {
        private Dict<string, ViewModel> mViewModels;

        private ViewModelManager()
        {
        }

        public void Initialize()
        {
            this.mViewModels = new Dict<string, ViewModel>();

            var rViewModelTypes = ViewModelTypes.Types;
            for (int i = 0; i < rViewModelTypes.Count; i++)
            {
                var rViewModelType = rViewModelTypes[i];
                var rAttrObjs = rViewModelType.GetCustomAttributes(typeof(ViewModelInitializeAttribute), false);
                if (rAttrObjs == null || rAttrObjs.Length == 0) continue;

                this.ReceiveViewModel(rViewModelType);
            }
        }

        public ViewModel ReceiveViewModel(Type rViewModelType)
        {
            var rViewModelClassName = rViewModelType.FullName;
            return this.ReceiveViewModel(rViewModelClassName);
        }

        public ViewModel ReceiveViewModel(string rViewModelClassName)
        {
            ViewModel rViewModel = null;
            if (!this.mViewModels.TryGetValue(rViewModelClassName, out rViewModel))
            {
                var rViewModelType = Type.GetType(rViewModelClassName);
                rViewModel = HotfixReflectAssists.Construct(rViewModelType) as ViewModel;
                rViewModel.Initialize();

                this.mViewModels.Add(rViewModelClassName, rViewModel);
            }
            return rViewModel;
        }

        public T ReceiveViewModel<T>() where T : ViewModel
        {
            var rViewModelType = typeof(T);
            return this.ReceiveViewModel(rViewModelType) as T;
        }
    }
}
