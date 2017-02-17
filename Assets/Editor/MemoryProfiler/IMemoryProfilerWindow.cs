using UnityEngine;
using System.Collections;
using UnityEditor;
using Treemap;

namespace MemoryProfilerWindow
{
    public abstract class IMemoryProfilerWindow : EditorWindow
    {
        public      Inspector   _inspector;
        protected   TreeMapView _treeMapView;

        public virtual void SelectThing(ThingInMemory thing) { }
        public virtual void SelectGroup(Group group) { }
    }
}

