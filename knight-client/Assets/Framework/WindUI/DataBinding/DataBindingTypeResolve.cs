using System;
using System.Collections.Generic;
using Knight.Framework.TypeResolve;
using Knight.Core;

namespace UnityEngine.UI
{
    public class DataBindingTypeResolve
    {
        public static string PathDot2Oblique(string rSrcPath)
        {
            return rSrcPath.Replace('.', '/');
        }

        public static string PathOblique2Dot(string rSrcPath)
        {
            return rSrcPath.Replace('/', '.');
        }

        public static List<string> GetAllModelPaths(GameObject rGo)
        {
            var rModelPathList = new List<string>();
            var rAllDataSources = rGo.GetComponentsInParent<DataSourceModel>(true);

            for (int i = 0; i < rAllDataSources.Length; i++)
            {
                var rClassName = rAllDataSources[i].ModelClass;
                if (!string.IsNullOrEmpty(rClassName))
                {
                    rModelPathList.AddRange(GetClassAllModelPaths(rClassName));
                }
            }
            return rModelPathList;
        }
        
        private static List<string> GetClassAllModelPaths(string rClassName)
        {
            var rModelPathList = new List<string>();
            var rType = TypeResolveManager.Instance.GetType(rClassName);
            if (rType == null)
            {
                Debug.LogErrorFormat("Has not type: {0} in register assemblies.", rClassName);
                return rModelPathList;
            }
            var rDataBindingAttr = rType.GetCustomAttribute<DataBindingAttribute>(false);
            if (rDataBindingAttr == null)
            {
                Debug.LogErrorFormat("Type: {0} not has attribute DataBindingAttribute.", rClassName);
                return rModelPathList;
            }

            var rAllFields = rType.GetFields(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllFields.Length; i++)
            {
                rModelPathList.Add(rClassName + "/" + rAllFields[i].Name);
            }
            var rAllProps = rType.GetProperties(ReflectionAssist.flags_public);
            for (int i = 0; i < rAllProps.Length; i++)
            {
                rModelPathList.Add(rClassName + "/" + rAllProps[i].Name);
            }
            return rModelPathList;
        }
    }
}
