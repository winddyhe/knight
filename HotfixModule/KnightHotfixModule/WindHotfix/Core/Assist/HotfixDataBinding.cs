using System;
using System.Collections.Generic;
using System.Reflection;
using WindHotfix.Game;
using Core;
using Framework.Hotfix;

namespace WindHotfix.Core
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HofixDataBindingAttribute : Attribute
    {
        public string   Name;
        public int      Index;

        public HofixDataBindingAttribute(string rName = "", int nIndex = -1)
        {
            this.Name  = rName;
            this.Index = nIndex;
        }
    }

    public class HotfixDataBindingAssist
    {
        public static void BindComponent(ComponentMBContainer rCompMBContainer)
        {
            Type rType = rCompMBContainer.GetType();
            if (rType == null) return;

            var rBindingFlags = BindingFlags.GetField | BindingFlags.SetField | BindingFlags.Public | BindingFlags.Instance;
            var rFiledInfos = rType.GetFields(rBindingFlags);
            for (int i = 0; i < rFiledInfos.Length; i++)
            {
                var rDataBindingAttr = rFiledInfos[i].GetCustomAttribute<HofixDataBindingAttribute>();
                if (rDataBindingAttr != null)
                {
                    UnityObject rUnityObject = null;
                    if (!string.IsNullOrEmpty(rDataBindingAttr.Name))
                        rUnityObject = rCompMBContainer.GetUnityObject(rDataBindingAttr.Name);
                    else
                        rUnityObject = rCompMBContainer.GetUnityObject(rDataBindingAttr.Index);
                    
                    // 如果属性没有，直接报错
                    if (rUnityObject == null)
                        UnityEngine.Debug.LogErrorFormat("Not find binding data, please check prefab and hofix script. {0}", rFiledInfos[i].Name);
                    else
                    {
                        if (rUnityObject.Type == rFiledInfos[i].FieldType.ToString())
                            UnityEngine.Debug.LogErrorFormat("Binding data type is not match. {0}", rFiledInfos[i].Name);
                        else
                            rFiledInfos[i].SetValue(rCompMBContainer, rUnityObject.Object);
                    }
                }
            }
        }
    }
}
