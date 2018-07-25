using System;
using System.Collections.Generic;
using Knight.Framework.TypeResolve;
using Knight.Core;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;

namespace UnityEngine.UI
{
    public class DataBindingTypeResolve
    {
        public static List<Type> ViewComponentBlackList = new List<Type>()
        {
            typeof(UnityEngine.CanvasRenderer),
            typeof(UnityEngine.UI.MemberBindingAbstract),
            typeof(UnityEngine.UI.MemberBindingOneWay)
        };
        
        public static List<string> GetAllViewModelPaths(IEnumerable<BindableMember<PropertyInfo>> rViewModelProps)
        {
            var rBindableMembers = rViewModelProps.Select(prop =>
            {
                var rDataSource = prop.PropOwner as ViewModelDataSource;
                return string.Format("{0}@{1}/{2} : {3}", rDataSource.Key, prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
            });
            return new List<string>(rBindableMembers);
        }

        public static List<string> GetAllViewPaths(GameObject rGo)
        {
            var rBindableMembers = GetViewProperties(rGo).Select(prop =>
            {
                return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
            });
            return new List<string>(rBindableMembers);
        }

        public static DataBindingProperty MakeViewDataBindingProperty(GameObject rGo, string rViewPath)
        {
            if (string.IsNullOrEmpty(rViewPath)) return null;

            var rViewPathStrs = rViewPath.Split('/');
            if (rViewPathStrs.Length < 2) return null;

            var rViewClassName = rViewPathStrs[0].Trim();
            var rViewProp = rViewPathStrs[1].Trim();

            var rViewPropStrs = rViewProp.Split(':');
            if (rViewPropStrs.Length < 1) return null;

            var rViewPropName = rViewPropStrs[0].Trim();

            var rViewDatabindingProp = rGo.GetComponents<Component>()
                .Where(comp => comp != null &&
                       comp.GetType().FullName.Equals(rViewClassName) &&
                       comp.GetType().GetProperty(rViewPropName) != null)
                .Select(comp =>
                {
                    return new DataBindingProperty(comp, rViewPropName);
                })
                .FirstOrDefault();
            return rViewDatabindingProp;
        }

        public static BindableEvent MakeViewDataBindingEvent(GameObject rGo, string rEventPath)
        {
            if (string.IsNullOrEmpty(rEventPath)) return null;

            var rEventPathStrs = rEventPath.Split('/');
            if (rEventPathStrs.Length < 2) return null;

            var rEventClassName = rEventPathStrs[0].Trim();
            var rEventName = rEventPathStrs[1].Trim();

            var rViewEventBindingComp = rGo.GetComponents<Component>()
                .Where(comp => comp != null && comp.GetType().FullName.Equals(rEventClassName)).FirstOrDefault();

            return GetBoundEvent(rEventName, rViewEventBindingComp);
        }

        private static IEnumerable<BindableMember<PropertyInfo>> GetViewProperties(GameObject rGo)
        {
            var rBindableMembers = rGo.GetComponents<Component>()
                .Where(comp => comp != null)
                .SelectMany(comp =>
                {
                    var rType = comp.GetType();
                    return rType
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Select(prop => new BindableMember<PropertyInfo>(comp, prop, rType));
                })
                .Where(prop => prop.Member.GetSetMethod(false) != null &&
                               prop.Member.GetGetMethod(false) != null &&
                               !ViewComponentBlackList.Contains(prop.ViewModelType) && 
                               !prop.Member.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any()
                      );

            return rBindableMembers;
        }
        
        public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelProperties(GameObject rGo, Type rViewPropType)
        {
            var rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                .Where(ds => ds != null &&
                       !string.IsNullOrEmpty(ds.ViewModelPath) &&
                       !string.IsNullOrEmpty(ds.Key))
                .SelectMany(ds =>
                {
                    var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                    return rType?
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                })
                .Where(prop => prop != null &&
                               prop.Member.PropertyType.Equals(rViewPropType) &&
                               prop.Member.GetSetMethod(false) != null &&
                               prop.Member.GetGetMethod(false) != null &&
                               !ViewComponentBlackList.Contains(prop.ViewModelType) &&
                               prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                      );
            
            return rBindableMembers;
        }

        public static BindableEvent GetBoundEvent(string rBoundEventName, Component rComp)
        {
            if (rComp == null || string.IsNullOrEmpty(rBoundEventName)) return null;

            var rCompType = rComp.GetType();
            var rBoundEvent = GetBindableEvents(rComp)?.FirstOrDefault();
            if (rBoundEvent == null)
            {
                throw new Exception(string.Format("Could not bind to event \"{0}\" on component \"{1}\".", rBoundEventName, rCompType));
            }
            return rBoundEvent;
        }

        public static string[] GetBindableEventPaths(GameObject rGo)
        {
            if (rGo == null) return new string[0];
            var rEventPaths = GetBindableEvents(rGo).Select(rEvent => 
            {
                return string.Format("{0}/{1}", rEvent.ComponentType.FullName, rEvent.Name);
            });
            return new List<string>(rEventPaths).ToArray();
        }

        public static BindableEvent[] GetBindableEvents(GameObject rGo)
        {
            if (rGo == null) return new BindableEvent[0];

            return rGo
                .GetComponents(typeof(Component))
                .Where(rComp => rComp != null)
                .SelectMany(GetBindableEvents)
                .ToArray();
        }

        private static IEnumerable<BindableEvent> GetBindableEvents(Component rComp)
        {
            if (rComp == null) return new BindableEvent[0];

            var rType = rComp.GetType();

            var rBindableEventsFromProperties = rType.GetProperties(ReflectionAssist.flags_public)
                .Where(rPropInfo => rPropInfo.PropertyType.IsSubclassOf(typeof(UnityEventBase)))
                .Where(rPropInfo => !rPropInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
                .Select(rPropInfo => new BindableEvent()
                {
                    UnityEvent      = (UnityEventBase)rPropInfo.GetValue(rComp, null),
                    Name            = rPropInfo.Name,
                    DeclaringType   = rPropInfo.DeclaringType,
                    ComponentType   = rComp.GetType(),
                    Component       = rComp
                });

            var rBindableEventsFromFields = rType.GetFields(ReflectionAssist.flags_public)
                .Where(rFieldInfo => rFieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                .Where(rFieldInfo => !rFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
                .Select(rFieldInfo => new BindableEvent()
                {
                    UnityEvent      = (UnityEventBase)rFieldInfo.GetValue(rComp),
                    Name            = rFieldInfo.Name,
                    DeclaringType   = rFieldInfo.DeclaringType,
                    ComponentType   = rComp.GetType(),
                    Component       = rComp
                });

            return rBindableEventsFromFields.Concat(rBindableEventsFromProperties);
        }
    }
}
