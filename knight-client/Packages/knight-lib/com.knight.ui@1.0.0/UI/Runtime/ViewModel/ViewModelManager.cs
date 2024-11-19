using Knight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Framework.UI
{
    public class ViewModelManager : TSingleton<ViewModelManager>
    {
        /// <summary>
        /// 全局的ViewModel
        /// </summary>
        private Dictionary<string, ViewModel> mGlobalViewModels;

        private ViewModelManager()
        {
        }

        public void Initialize()
        {
            this.mGlobalViewModels = new Dictionary<string, ViewModel>();

            var rViewModelTypes = ViewModelTypes.Types;
            foreach (var rViewModelType in rViewModelTypes)
            {
                var rAttrObjs = rViewModelType.GetCustomAttributes(typeof(ViewModelInitializeAttribute), false);
                if (rAttrObjs != null && rAttrObjs.Length > 0)
                {
                    var rViewModelClassName = rViewModelType.FullName;
                    var rViewModel = this.ReceiveGlobalViewModel(rViewModelClassName);
                    if (rViewModel != null)
                    {
                        this.mGlobalViewModels.Add(rViewModelClassName, rViewModel);
                    }
                }
            }
        }

        public T ReceiveGlobalViewModel<T>() where T : ViewModel, new()
        {
            var rViewModelType = typeof(T);
            if (!this.mGlobalViewModels.TryGetValue(rViewModelType.FullName, out var rViewModel))
            {
                rViewModel = new T();
                this.mGlobalViewModels.Add(rViewModelType.FullName, rViewModel);
            }
            return rViewModel as T;
        }

        public ViewModel ReceiveGlobalViewModel(string rViewModelClassName)
        {
            if (!this.mGlobalViewModels.TryGetValue(rViewModelClassName, out var rViewModel))
            {
                var rViewModelType = TypeResolveManager.Instance.GetType(rViewModelClassName);
                if (rViewModelType == null) return null;

                rViewModel = ReflectTool.Construct(rViewModelType) as ViewModel;
                if (rViewModel == null) return null;
            }
            return rViewModel;
        }

        public ViewModel CreateViewModel(string rViewModelClassName)
        {
            var rViewModelType = TypeResolveManager.Instance.GetType(rViewModelClassName);
            if (rViewModelType == null) return null;

            var rViewModel = ReflectTool.Construct(rViewModelType) as ViewModel;
            if (rViewModel == null) return null;

            return rViewModel;
        }
    }
}
