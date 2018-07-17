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
        public string               ViewPath;
        [Dropdown("ModelPaths")]
        public string               ModelPath;

        public DataBindingProperty  ViewProp;
        public DataBindingProperty  ViewModelProp;
    }
}
