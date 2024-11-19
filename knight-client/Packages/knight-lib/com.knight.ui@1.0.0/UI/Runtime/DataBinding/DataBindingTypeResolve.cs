using Knight.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Knight.Framework.UI
{
    public class DataBindingTypeResolve
    {
        public static List<string> AssemblyWhiteList = new List<string>()
        {
            "UnityEngine.UI",
            "Knight.UI",
            "UnityEngine.UIModule",
            "UnityEngine.CoreModule",
        };
        public static List<Type> ViewComponentBlackList = new List<Type>()
        {
            typeof(CanvasRenderer),
#if UNITY_EDITOR
            typeof(DataBindingAbstract),
#endif
            typeof(ViewModelDataSource),
            typeof(Light),
            typeof(StandaloneInputModule),
            typeof(MeshRenderer),
        };

        public static List<string> GetViewPaths(GameObject rGameObject)
        {
            var rBindableMembers = GetViewProperties(rGameObject).Select(prop =>
            {
                return string.Format("{0}/{1} : {2}", prop.ObjectType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
            });
            return new List<string>(rBindableMembers);
        }

        public static List<string> GetViewModelPaths(IEnumerable<BindableMember<PropertyInfo>> rViewModelProps)
        {
            var rBindableMembers = rViewModelProps
                .Where(prop => prop != null)
                .Select(prop =>
                {
                    var rDataSource = prop.Owner as ViewModelDataSource;
                    var rRealPropType = $"{prop.Member.PropertyType.Name}";
                    if (prop.Member.PropertyType.IsGenericType && prop.Member.PropertyType.GetGenericArguments().Length == 1 && 
                        typeof(IList).IsAssignableFrom(prop.Member.PropertyType.GetGenericTypeDefinition()))
                    {
                        var rTypeName = prop.Member.PropertyType.Name.Split('`')[0];
                        rRealPropType = $"{rTypeName}<{prop.Member.PropertyType.GetGenericArguments()[0].Name}>";
                    }
                    if (rDataSource != null)
                    {
                        if (string.IsNullOrEmpty(rDataSource.Key))
                            return string.Format("{0}/{1} : {2}", prop.ObjectType.FullName, prop.MemberName, rRealPropType);
                        else
                            return string.Format("{0}${1}/{2} : {3}", rDataSource.Key, prop.ObjectType.FullName, prop.MemberName, rRealPropType);
                    }
                    else
                    {
                        return string.Format("ListTemplate@{0}/{1} : {2}", prop.ObjectType.FullName, prop.MemberName, rRealPropType);
                    }
                });
            return new List<string>(rBindableMembers);
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
                               !ViewComponentBlackList.Contains(prop.ObjectType) &&
                               !prop.Member.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any()
                      );
            return rBindableMembers;
        }

        public static BindableProperty MakeViewDataBindingProperty(GameObject rGo, string rViewPath)
        {
            if (string.IsNullOrEmpty(rViewPath)) return null;
            var rViewPathStrs = rViewPath.Split('/');
            if (rViewPathStrs.Length < 2) return null;

            var rViewClassName = rViewPathStrs[0].Trim();
            var rViewProp = rViewPathStrs[1].Trim();

            var rViewPropStrs = rViewProp.Split(':');
            if (rViewPropStrs.Length < 1) return null;

            var rViewPropName = rViewPropStrs[0].Trim();
            var rViewPropValue = rViewPropStrs[1].Trim();
            var rViewDatabindingProp = rGo.GetComponents<Component>()
                .Where(comp => comp != null &&
                               comp.GetType().FullName.Equals(rViewClassName) &&
                               comp.GetType().GetProperty(rViewPropName) != null)
                .Select(comp =>
                {
                    return new BindableProperty(comp, rViewPropValue, rViewPropName);
                })
                .FirstOrDefault();
            return rViewDatabindingProp;
        }

        public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelProperties(GameObject rGo, Type rViewPropType, bool bIsListTemplate)
        {
            if (!bIsListTemplate)
            {
                IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = null;
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                    .Where(ds => ds != null &&
                                 !string.IsNullOrEmpty(ds.ViewModelPath))
                    .SelectMany(ds =>
                    {
                        var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                        return GetProperties(rType, BindingFlags.Instance | BindingFlags.Public)
                                .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                    })
                    .Where(prop => prop != null &&
                                   prop.Member.PropertyType.Equals(rViewPropType) &&
                                   ((prop.Member.GetSetMethod(false) != null &&
                                     prop.Member.GetGetMethod(false) != null &&
                                     prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                                    ) ||
                                    (prop.Member.GetGetMethod(false) != null &&
                                     prop.Member.GetCustomAttributes(typeof(DataBindingRelatedAttribute), true).Any()
                                    )
                                   )
                          );
                return rBindableMembers;
            }
            else
            {
                IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = null;
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSourceListTemplate>(true)
                    .Where(ds => ds != null &&
                                 !string.IsNullOrEmpty(ds.TemplatePath))
                    .SelectMany(ds =>
                    {
                        var rType = TypeResolveManager.Instance.GetType(ds.TemplatePath);
                        return GetProperties(rType, BindingFlags.Instance | BindingFlags.Public)
                                .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                    })
                    .Where(prop => prop != null &&
                                   prop.Member.PropertyType.Equals(rViewPropType) &&
                                   ((prop.Member.GetSetMethod(false) != null &&
                                     prop.Member.GetGetMethod(false) != null &&
                                     prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                                    ) ||
                                    (prop.Member.GetGetMethod(false) != null &&
                                     prop.Member.GetCustomAttributes(typeof(DataBindingRelatedAttribute), true).Any()
                                    )
                                   )
                          );
                return rBindableMembers;
            }
        }

        public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelListTemplateProperties(GameObject rGo)
        {
            IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = null;
            rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                .Where(ds => ds != null &&
                             !string.IsNullOrEmpty(ds.ViewModelPath))
                .SelectMany(ds =>
                {
                    var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                    return GetProperties(rType, BindingFlags.Instance | BindingFlags.Public)
                            .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                })
                .Where(prop => prop != null &&
                               prop.Member.PropertyType.IsGenericType &&
                               prop.Member.PropertyType.GetGenericArguments().Length == 1 &&
                               typeof(IList).IsAssignableFrom(prop.Member.PropertyType.GetGenericTypeDefinition()) &&
                               ((prop.Member.GetSetMethod(false) != null &&
                                 prop.Member.GetGetMethod(false) != null &&
                                 prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                                ) ||
                                (prop.Member.GetGetMethod(false) != null &&
                                 prop.Member.GetCustomAttributes(typeof(DataBindingRelatedAttribute), true).Any()
                                )
                               )
                      );
            return rBindableMembers;
        }

        private static List<PropertyInfo> GetProperties(Type rType, BindingFlags rBindingFlags)
        {
            var rProperties = new List<PropertyInfo>();
            var rTempType = rType;
            HashSet<string> rPropertyHashSet = new HashSet<string>();
            PropertyInfo[] rTempProperties;
            int nTempPropertiesCount;
            PropertyInfo rTempProperty;
            string rTempPropertyName;
            while (rTempType != null && rTempType.Name != "ViewModel")
            {
                rTempProperties = rTempType.GetProperties(rBindingFlags);
                nTempPropertiesCount = rTempProperties.Length;
                for (int i = 0; i < nTempPropertiesCount; i++)
                {
                    rTempProperty = rTempProperties[i];
                    rTempPropertyName = rTempProperty.Name;
                    if (rPropertyHashSet.Contains(rTempPropertyName))
                    {
                        continue;
                    }

                    rPropertyHashSet.Add(rTempPropertyName);
                    rProperties.Add(rTempProperty);
                }

                rTempType = rTempType.BaseType;
            }

            return rProperties;
        }

        public static List<string> GetAllViewModels()
        {
            var rTypeNames = TypeResolveManager.Instance.GetTypes("Game.Hotfix")
                   .Where(rType => rType != null &&
                                   rType.GetCustomAttributes(typeof(DataBindingAttribute), true).Any() &&
                                   IsBaseTypeEquals(rType, "Knight.Framework.UI.ViewModel"))
                   .Select(rType =>
                   {
                       return rType.FullName;
                   });
            return new List<string>(rTypeNames);
        }

        private static bool IsBaseTypeEquals(Type rType, string rTypeName)
        {
            var rTempType = rType.BaseType;
            bool bResult = false;
            while (rTempType != null)
            {
                if (rTempType.FullName.Equals(rTypeName))
                {
                    bResult = true;
                    break;
                }
                rTempType = rTempType.BaseType;
            }
            return bResult;
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

            var rBindableEventsFromProperties = rType.GetProperties(ReflectTool.flags_public)
                .Where(rPropInfo => rPropInfo.PropertyType.IsSubclassOf(typeof(UnityEventBase)))
                .Where(rPropInfo => !rPropInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
                .Select(rPropInfo => new BindableEvent()
                {
                    UnityEvent = (UnityEventBase)rPropInfo.GetValue(rComp, null),
                    Name = rPropInfo.Name,
                    DeclaringType = rPropInfo.DeclaringType,
                    ComponentType = rComp.GetType(),
                    Component = rComp
                });

            var rBindableEventsFromFields = rType.GetFields(ReflectTool.flags_public)
                .Where(rFieldInfo => rFieldInfo.FieldType.IsSubclassOf(typeof(UnityEventBase)))
                .Where(rFieldInfo => !rFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), true).Any())
                .Select(rFieldInfo => new BindableEvent()
                {
                    UnityEvent = (UnityEventBase)rFieldInfo.GetValue(rComp),
                    Name = rFieldInfo.Name,
                    DeclaringType = rFieldInfo.DeclaringType,
                    ComponentType = rComp.GetType(),
                    Component = rComp
                });

            return rBindableEventsFromFields.Concat(rBindableEventsFromProperties);
        }

        public static string[] GetViewModelBindingEvents(GameObject rGo, bool bIsListTemplate)
        {
            var rBindableEvents = new List<string>();

            var rViewModelContainer = rGo.GetComponentInParent<ViewControllerDataSource>();
            if (rViewModelContainer == null) return rBindableEvents.ToArray();

            var rViewModelType = TypeResolveManager.Instance.GetType(rViewModelContainer.ViewControllerClass);
            if (rViewModelType == null) return rBindableEvents.ToArray();

            var rAllMethods = rViewModelType.GetMethods(ReflectTool.flags_method_inst);
            for (int i = 0; i < rAllMethods.Length; i++)
            {
                var rAttrs = rAllMethods[i].GetCustomAttributes(false);
                if (rAttrs.Length == 0) continue;

                var rDataBindingAttr = rAttrs[0] as DataBindingEventAttribute;
                if (rDataBindingAttr == null) continue;

                if (bIsListTemplate && !rDataBindingAttr.IsListEvent) continue;
                if (!bIsListTemplate &&  rDataBindingAttr.IsListEvent) continue;

                rBindableEvents.Add(string.Format("{0}/{1}", rViewModelContainer.ViewControllerClass, rAllMethods[i].Name));
            }
            return rBindableEvents.ToArray();
        }

        public static List<string> GetAllViews()
        {
            var rTypeNames = TypeResolveManager.Instance.GetTypes("Game.Hotfix")
                .Where(rType => rType != null &&
                                rType.BaseType?.FullName == "Knight.Framework.UI.ViewController")
                .Select(rType =>
                {
                    return rType.FullName;
                });
            return new List<string>(rTypeNames);
        }

        public static Component GetViewEventComponent(GameObject rGo, string rEventPath)
        {
            if (string.IsNullOrEmpty(rEventPath)) return null;

            var rEventPathStrs = rEventPath.Split('/');
            if (rEventPathStrs.Length < 2) return null;

            var rViewClassName = rEventPathStrs[0].Trim();
            var rViewEventName = rEventPathStrs[1].Trim();

            var rViewEventComp = rGo.GetComponents<Component>()
                .Where(comp => comp != null &&
                               comp.GetType().FullName.Equals(rViewClassName))
                .FirstOrDefault();
            return rViewEventComp;
        }
    }
}
