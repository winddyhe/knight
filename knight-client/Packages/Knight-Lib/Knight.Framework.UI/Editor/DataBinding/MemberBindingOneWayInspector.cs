using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using NaughtyAttributes.Editor;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(MemberBindingOneWay), true)]
    public class MemberBindingOneWayInspector : MemberBindingAbstractInspector
    {
    }
}
