using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
    public partial class MemberBindingAbstract : MonoBehaviour
    {
        [Dropdown("ViewPaths")]
        public string                       ViewPath;
        [Dropdown("ModelPaths")]
        public string                       ViewModelPath;

        public DataBindingProperty          ViewProp;
        public DataBindingProperty          ViewModelProp;

        public DataBindingPropertyWatcher   ViewModelPropertyWatcher;

        public void SyncFromViewModel()
        {
            var rValue = this.ViewModelProp?.GetValue();
            this.ViewProp?.SetValue(rValue);
        }

        public void SyncFromView()
        {
            var rValue = this.ViewProp?.GetValue();
            this.ViewModelProp?.SetValue(rValue);
        }
    }
}
