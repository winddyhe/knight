using System;
using System.Collections.Generic;
using Knight.Framework.TypeResolve;
using Knight.Core;

namespace UnityEngine.UI
{
    public class DataBindingTypeResolve
    {
        public static List<Type> ViewComponentBlackList = new List<Type>()
        {
            typeof(UnityEngine.CanvasRenderer),
            typeof(UnityEngine.UI.DataBindingOneWay)
        };
            
        public static string PathDot2Oblique(string rSrcPath)
        {
            return rSrcPath.Replace('.', '/');
        }

        public static string PathOblique2Dot(string rSrcPath)
        {
            return rSrcPath.Replace('/', '.');
        }

        public static bool CheckViewComponentBlackList(Type rType)
        {
            bool bIsInBlackList = false;
            for (int i = 0; i < ViewComponentBlackList.Count; i++)
            {
                if (rType.Equals(ViewComponentBlackList[i]))
                {
                    bIsInBlackList = true;
                    break;
                }
            }
            return bIsInBlackList;
        }

        public static List<ModelDataItem> GetAllModelPaths(GameObject rGo)
        {
            var rModelDataList = new List<ModelDataItem>();
            var rAllDataSources = rGo.GetComponentsInParent<DataSourceModel>(true);

            for (int i = 0; i < rAllDataSources.Length; i++)
            {
                var rClassName = rAllDataSources[i].ViewModelClass;
                if (!string.IsNullOrEmpty(rClassName))
                {
                    rModelDataList.AddRange(GetClassAllModelPaths(rAllDataSources[i], rClassName));
                }
            }
            return rModelDataList;
        }

        public static List<ViewDataItem> GetAllViewPaths(ModelDataItem rDataModelItem, GameObject rGo)
        {
            var rViewDataList = new List<ViewDataItem>();
            var rAllComps = rGo.GetComponents<Component>();
            for (int i = 0; i < rAllComps.Length; i++)
            {
                Type rCompType = rAllComps[i].GetType();
                if (CheckViewComponentBlackList(rCompType)) continue;
                rViewDataList.AddRange(GetComponentAllViewPaths(rDataModelItem, rAllComps[i]));
            }
            return rViewDataList;
        }
        
        private static List<ModelDataItem> GetClassAllModelPaths(DataSourceModel rDataSourceModel, string rClassName)
        {
            var rModelDataList = new List<ModelDataItem>();
            var rType = TypeResolveManager.Instance.GetType(rClassName);
            if (rType == null)
            {
                Debug.LogErrorFormat("Has not type: {0} in register assemblies.", rClassName);
                return rModelDataList;
            }
            var rDataBindingAttr = rType.GetCustomAttribute<DataBindingAttribute>(false);
            if (rDataBindingAttr == null)
            {
                Debug.LogErrorFormat("Type: {0} not has attribute DataBindingAttribute.", rClassName);
                return rModelDataList;
            }

            var rAllFields = rType.GetFields(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllFields.Length; i++)
            {
                var rModelDataItem = new ModelDataItem()
                {
                    DataSource  = rDataSourceModel,
                    Path        = rClassName + "/" + rAllFields[i].Name,
                    ClassName   = rClassName,
                    VaribleType = rAllFields[i].FieldType,
                    VaribleName = rAllFields[i].Name
                };
                rModelDataList.Add(rModelDataItem);
            }
            var rAllProps = rType.GetProperties(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllProps.Length; i++)
            {
                var rModelDataItem = new ModelDataItem()
                {
                    DataSource  = rDataSourceModel,
                    Path        = rClassName + "/" + rAllProps[i].Name,
                    ClassName   = rClassName,
                    VaribleType = rAllProps[i].PropertyType,
                    VaribleName = rAllProps[i].Name
                };
                rModelDataList.Add(rModelDataItem);
            }
            return rModelDataList;
        }

        private static List<ViewDataItem> GetComponentAllViewPaths(ModelDataItem rDataModelItem, Component rComp)
        {
            var rViewDataList = new List<ViewDataItem>();
            if (rComp == null)
            {
                return rViewDataList;
            }

            var rCompType = rComp.GetType();
            var rAllFields = rCompType.GetFields(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllFields.Length; i++)
            {
                if (!rAllFields[i].FieldType.Equals(rDataModelItem.VaribleType)) continue;

                var rViewDataItem   = new ViewDataItem()
                {
                    ViewComp    = rComp,
                    ClassName   = rCompType.FullName,
                    VaribleType = rAllFields[i].FieldType,
                    Path        = rCompType.FullName + "/" + rAllFields[i].Name,
                    VaribleName = rAllFields[i].Name
                };
                rViewDataList.Add(rViewDataItem);
            }
            var rAllProps = rCompType.GetProperties(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllProps.Length; i++)
            {
                if (!rAllProps[i].PropertyType.Equals(rDataModelItem.VaribleType)) continue;

                var rViewDataItem   = new ViewDataItem()
                {
                    ViewComp    = rComp,
                    ClassName   = rCompType.FullName,
                    VaribleType = rAllProps[i].PropertyType,
                    Path        = rCompType.FullName + "/" + rAllProps[i].Name,
                    VaribleName = rAllProps[i].Name
                };
                rViewDataList.Add(rViewDataItem);
            }
            return rViewDataList;
        }
    }
}
