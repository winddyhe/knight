using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using NaughtyAttributes.Editor;
using UnityEditor;

namespace Knight.Framework.TinyMode.UI.Editor
{
    [CustomEditor(typeof(MemberBindingOneWay), true)]
    public class MemberBindingOneWayInspector : MemberBindingAbstractInspector
    {
    }
}
