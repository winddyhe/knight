using System;
using System.Collections.Generic;

namespace WindHotfix.GamePlay
{
    public class SubSystem
    {
        public virtual void ExecuteComponents(List<Component> rComps) { }
        public virtual Component GenerateComponent() { return null; }
        public virtual void Update() { }
    }
}
