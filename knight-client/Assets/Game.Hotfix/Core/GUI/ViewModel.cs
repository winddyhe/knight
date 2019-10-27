using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.UI;
using Knight.Core;
using System.Reflection;

namespace Knight.Hotfix.Core
{
    public class ViewModelTypes : HotfixTypeSearchDefault<ViewModel>
    {
    }

    [HotfixTSIgnore]
    public class ViewModel
    {
        private Dict<string, List<string>>  mPropMaps = new Dict<string, List<string>>();

        public  Action<string>              PropChangedHandler;

        public void Initialize()
        {
            this.mPropMaps.Clear();

            var rType = this.GetType();
            foreach (var rPropInfo in rType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var rAttribute = rPropInfo.GetCustomAttribute<DataBindingRelatedAttribute>();
                if (rAttribute == null) continue;

                if (rAttribute.RelatedProps != null)
                {
                    if (string.IsNullOrEmpty(rAttribute.RelatedProps)) continue;

                    var rRelatedProps = rAttribute.RelatedProps.Split(',');
                    for (int i = 0; i < rRelatedProps.Length; i++)
                    {
                        List<string> rProps = null;
                        if (!this.mPropMaps.TryGetValue(rRelatedProps[i].Trim(), out rProps))
                        {
                            rProps = new List<string>();
                            this.mPropMaps.Add(rRelatedProps[i].Trim(), rProps);
                        }
                        rProps.Add(rPropInfo.Name);
                    }
                }
            }
        }

        public void PropChanged(string rPropName)
        {
            this.PropChangedHandler?.Invoke(rPropName);
            List<string> rRelatedProps = null;
            if (this.mPropMaps.TryGetValue(rPropName, out rRelatedProps))
            {
                for (int i = 0; i < rRelatedProps.Count; i++)
                {
                    this.PropChangedHandler?.Invoke(rRelatedProps[i]);
                }
            }
        }
    }
}
