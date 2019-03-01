using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class BindableEvent
    {
        public Component        Component       { get; set; }
        public UnityEventBase   UnityEvent      { get; set; }
        public string           Name            { get; set; }
        public Type             DeclaringType   { get; set; }
        public Type             ComponentType   { get; set; }
    }
}
