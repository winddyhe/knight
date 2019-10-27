using System;
using System.Collections.Generic;
using Knight.Core;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;
using System.Collections;

namespace UnityEngine.UI
{
    public class DataBindingTypeResolve
    {
        public static List<Type> ViewComponentBlackList = new List<Type>()
        {
            typeof(UnityEngine.CanvasRenderer),
            typeof(UnityEngine.UI.MemberBindingAbstract),
            typeof(UnityEngine.UI.MemberBindingOneWay),
            typeof(UnityEngine.UI.MemberBindingTwoWay),
            typeof(UnityEngine.UI.EventBinding)
        };
        
        public static List<string> GetAllViewModelPaths(IEnumerable<BindableMember<PropertyInfo>> rViewModelProps)
        {
            var rBindableMembers = rViewModelProps
                .Where(prop => prop != null)
                .Select(prop =>
                {
                    var rDataSource = prop.PropOwner as ViewModelDataSource;
                    if (rDataSource != null)
                    {
                        return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
                    }
                    else
                    {
                        return string.Format("ListTemplate@{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
                    }
                });
            return new List<string>(rBindableMembers);
        }

        public static List<string> GetAllConvertMethodPaths(List<BindableMember<MethodInfo>> rViewModelMethods)
        {
            var rBindableMembers = rViewModelMethods
                .Where(method => method != null)
                .Select(method =>
                {
                    return string.Format("{0}/{1}", method.ViewModelType.FullName, method.MemberName);
                });
            return new List<string>(rBindableMembers);
        }

        public static List<string> GetListItemAllViewModelPaths(IEnumerable<BindableMember<PropertyInfo>> rViewModelProps)
        {
            var rBindableMembers = rViewModelProps
            .Select(prop =>
            {
                if (prop == null) return "";
                return string.Format("{0}/{1} : {2}", prop.ViewModelType.FullName, prop.MemberName, prop.Member.PropertyType.Name);
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

        public static List<string> GetAllViewModels()
        {
            var rTypeNames = TypeResolveManager.Instance.GetAllTypes(true)
                   .Where(rType => rType != null &&
                                   rType.GetCustomAttributes(typeof(DataBindingAttribute), true).Any() &&
                                   rType.BaseType?.FullName == "Knight.Hotfix.Core.ViewModel")
                   .Select(rType =>
                   {
                       return rType.FullName;
                   });
            return new List<string>(rTypeNames);
        }

        public static List<string> GetAllViews()
        {
            var rTypeNames = TypeResolveManager.Instance.GetAllTypes(true)
                .Where(rType => rType != null &&
                                rType.BaseType?.FullName == "Knight.Hotfix.Core.ViewController")
                .Select(rType => 
                {
                    return rType.FullName;
                });
            return new List<string>(rTypeNames);
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
        
        public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelProperties(GameObject rGo, Type rViewPropType, bool bIsList)
        {
            IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = null;
            if (bIsList)
            {
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSourceTemplate>(true)
                    .Where(ds => ds != null &&
                           !string.IsNullOrEmpty(ds.ViewModelPath) &&
                           !string.IsNullOrEmpty(ds.TemplatePath))
                    .SelectMany(ds =>
                    {
                        if (ds == null) return null;
                        var rType = TypeResolveManager.Instance.GetType(ds.TemplatePath);
                        if (rType == null) return null;
                        return rType?
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                    })
                    ?.Where(prop => prop != null &&
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
            }
            else
            {
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                    .Where(ds => ds != null &&
                                 !string.IsNullOrEmpty(ds.ViewModelPath))
                    .SelectMany(ds =>
                    {
                        var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                        return rType?
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
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
            }
            return rBindableMembers;
        }

        public static IEnumerable<BindableMember<PropertyInfo>> GetViewModelProperties(GameObject rGo, Type rViewPropType, Type rViewModelType, bool bIsList)
        {
            IEnumerable<BindableMember<PropertyInfo>> rBindableMembers = null;
            if (bIsList)
            {
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSourceTemplate>(true)
                    .Where(ds => ds != null &&
                           !string.IsNullOrEmpty(ds.ViewModelPath) &&
                           !string.IsNullOrEmpty(ds.TemplatePath))
                    .SelectMany(ds =>
                    {
                        var rType = TypeResolveManager.Instance.GetType(ds.TemplatePath);
                        return rType?
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
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
            }
            else
            {
                rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                    .Where(ds => ds != null &&
                                 !string.IsNullOrEmpty(ds.ViewModelPath))
                    .SelectMany(ds =>
                    {
                        var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                        return rType?
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
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
            }
            return rBindableMembers;
        }

        public static IEnumerable<BindableMember<MethodInfo>> GetViewModelConvertMethods(Type rViewPropType, bool bIsList)
        {
            IEnumerable<BindableMember<MethodInfo>> rBindableMembers = null;
            if (bIsList)
            {
                rBindableMembers = TypeResolveManager.Instance.GetAllTypes(true)
                    .Where(type => type != null &&
                           type.GetCustomAttributes(typeof(DataBindingConvertAttribute), true).Any())
                    .SelectMany(type =>
                    {
                        return type?
                                .GetMethods(ReflectionAssist.flags_method_static)
                                .Select(method => new BindableMember<MethodInfo>(null, method, type));
                    })
                    .Where(method => method != null &&
                                     method.Member.GetParameters().Length == 1 &&
                                     method.Member.ReturnType.Equals(rViewPropType) &&
                                     method.Member.GetCustomAttributes(false).Any(attr => (attr as DataBindingConvertAttribute) != null)
                    );
            }
            else
            {
                rBindableMembers = TypeResolveManager.Instance.GetAllTypes(true)
                    .Where(type => type != null &&
                           type.GetCustomAttributes(typeof(DataBindingConvertAttribute), true).Any())
                    .SelectMany(type =>
                    {
                        return type?
                                .GetMethods(ReflectionAssist.flags_method_static)
                                .Select(method => new BindableMember<MethodInfo>(null, method, type));
                    })
                    .Where(method => method != null &&
                                     method.Member.GetParameters().Length == 1 &&
                                     method.Member.ReturnType.Equals(rViewPropType) &&
                                     method.Member.GetCustomAttributes(false).Any(attr => (attr as DataBindingConvertAttribute) != null)
                    );
            }
            return rBindableMembers;
        }
        
        public static IEnumerable<BindableMember<PropertyInfo>> GetListItemViewModelProperties(GameObject rGo, Type rViewPropType)
        { 
            var rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSourceTemplate>(true)
                .Where(ds => ds != null &&
                             !string.IsNullOrEmpty(ds.TemplatePath))
                .DefaultIfEmpty()
                .SelectMany(ds =>
                {
                    var rType = TypeResolveManager.Instance.GetType(ds.TemplatePath);
                    if (rType == null) return new BindableMember<PropertyInfo>[0];
                    return rType
                                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .DefaultIfEmpty()
                                .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                })
                .DefaultIfEmpty()
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
                       )
                .DefaultIfEmpty();

            return rBindableMembers;
        }

        public static IEnumerable<BindableMember<PropertyInfo>> GetListViewModelProperties(GameObject rGo)
        {
            var rBindableMembers = rGo.GetComponentsInParent<ViewModelDataSource>(true)
                .Where(ds => ds != null &&
                       !string.IsNullOrEmpty(ds.ViewModelPath))
                .SelectMany(ds =>
                {
                    var rType = TypeResolveManager.Instance.GetType(ds.ViewModelPath);
                    if (rType == null) return new BindableMember<PropertyInfo>[0];
                    return rType?
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .DefaultIfEmpty()
                            .Select(prop => new BindableMember<PropertyInfo>(ds, prop, rType));
                })
                .DefaultIfEmpty()
                .Where(prop => {
                    if (prop == null || prop.Member == null) return false;
                    var rPropType = ITypeRedirect.GetRedirectType(prop.Member.PropertyType);
                    return 
                        prop != null &&
                        rPropType.IsGenericType &&
                        typeof(IList).IsAssignableFrom(rPropType.GetGenericTypeDefinition()) &&
                        ((prop.Member.GetSetMethod(false) != null &&
                          prop.Member.GetGetMethod(false) != null &&
                          prop.Member.GetCustomAttributes(typeof(DataBindingAttribute), true).Any()
                         ) ||
                         (prop.Member.GetGetMethod(false) != null &&
                          prop.Member.GetCustomAttributes(typeof(DataBindingRelatedAttribute), true).Any()
                         )
                        );
                }
            )
            .DefaultIfEmpty();

            return rBindableMembers;
        }

        public static BindableEvent GetBoundEvent(string rBoundEventName, Component rComp)
        {
            if (rComp == null || string.IsNullOrEmpty(rBoundEventName)) return null;

            var rCompType = rComp.GetType();
            var rBoundEvent = GetBindableEvents(rComp)?.FirstOrDefault();
            if (rBoundEvent == null)
            {
                Debug.LogErrorFormat("Could not bind to event \"{0}\" on component \"{1}\".", rBoundEventName, rCompType);
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

        public static string[] GetViewModelBindingEvents(GameObject rGo)
        {
            var rBindableEvents = new List<string>();

            var rViewModelContainer = rGo.GetComponentInParent<ViewControllerContainer>();
            if (rViewModelContainer == null) return rBindableEvents.ToArray();

            var rViewModelType = TypeResolveManager.Instance.GetType(rViewModelContainer.ViewControllerClass);
            var rAllMethods = rViewModelType.GetMethods(ReflectionAssist.flags_method_inst);
            for (int i = 0; i < rAllMethods.Length; i++)
            {
                var rAttrs = rAllMethods[i].GetCustomAttributes(false);
                if (rAttrs.Length == 0) continue;

                var rDataBindingAttr = rAttrs[0] as DataBindingAttribute;
                if (rDataBindingAttr == null) continue;

                rBindableEvents.Add(string.Format("{0}/{1}", rViewModelContainer.ViewControllerClass, rAllMethods[i].Name));
            }
            return rBindableEvents.ToArray();
        }
    }
}
