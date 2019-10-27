using Knight.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnityEngine.UI
{
#pragma warning restore 219
    public class DataBindingAOTHelper
    {
        public void EnsureGenericTypes()
        {
            var strEventBinder = new UnityEventBinder<string>(null, null);
            var floatEventBinder = new UnityEventBinder<float>(null, null);
            var boolEventBinder = new UnityEventBinder<bool>(null, null);
            var intEventBinder = new UnityEventBinder<int>(null, null);
            var vector2EventBinder = new UnityEventBinder<Vector2>(null, null);
            var vector3EventBinder = new UnityEventBinder<Vector3>(null, null);
            var colorEventBinder = new UnityEventBinder<Color>(null, null);
            var baseEventDataEventBinder = new UnityEventBinder<EventArg>(null, null);
        }
    }
#pragma warning restore 219
}
