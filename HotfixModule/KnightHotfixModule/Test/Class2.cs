using Framework.Hotfix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Core;

namespace KnightHotfixModule.Test
{
    public class Class2 : MonoBehaviourProxy
    {
        private Dictionary<UnityEngine.Object, System.Action<UnityEngine.Object>> mEvents;

        public override void SetObjects(List<UnityEngine.Object> rObjs)
        {
            base.SetObjects(rObjs);

            mEvents = new Dictionary<UnityEngine.Object, Action<UnityEngine.Object>>();
            for (int i = 0; i < this.Objects.Count; i++)
            {
                mEvents.Add(this.Objects[i], (rTarget) =>
                {
                    Debug.LogError("You clicked..." + rTarget.name + ", " + rTarget.GetType());
                });
            }
        }

        public override void Start()
        {
        }

        public override void OnUnityEvent(UnityEngine.Object rTarget)
        {
            //Debug.LogError("You clicked..." + rTarget.name + ", " + rTarget.GetType());
            foreach (var rItem in mEvents)
            {
                rItem.Value(rTarget);
            }
        }
    }
}
