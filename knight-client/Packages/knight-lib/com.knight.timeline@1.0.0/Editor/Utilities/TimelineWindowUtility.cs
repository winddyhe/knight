using System.Reflection;
using UnityEngine;

namespace Knight.Framework.Timeline.Editor
{
    public static class TimelineWindowUtility
    {
        public static PropertyInfo InstancePropertyInfo;
        public static PropertyInfo StatePropertyInfo;
        static TimelineWindowUtility()
        {
            var rTimelineWindowTyepAssembly = Assembly.Load("Unity.Timeline.Editor");
            Debug.Assert(rTimelineWindowTyepAssembly != null);
            var rTimelineWindowTyep = rTimelineWindowTyepAssembly.GetType("UnityEditor.Timeline.TimelineWindow");
            Debug.Assert(rTimelineWindowTyep != null);
            InstancePropertyInfo = rTimelineWindowTyep.GetProperty("instance");
            Debug.Assert(InstancePropertyInfo != null);
            StatePropertyInfo = rTimelineWindowTyep.GetProperty("state");
            Debug.Assert(StatePropertyInfo != null);
        }
        public static bool CheckStateNotNull()
        {
            var rInstance = InstancePropertyInfo.GetValue(null);
            if (rInstance == null)
            {
                return false;
            }
            var rState = StatePropertyInfo.GetValue(rInstance);
            return rState != null;
        }
    }
}
