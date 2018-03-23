using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditorGUIAttribute : Attribute
    {
        public string       HeadText;
        public int          Width;
        
        public EditorGUIAttribute()
        {
            this.HeadText   = string.Empty;
            this.Width      = 0;
        }

        public EditorGUIAttribute(string rHeadText, int nWidth)
        {
            this.HeadText   = rHeadText;
            this.Width      = nWidth;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditorGUIIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class EditorGUIEnableAttribute : Attribute { }

    public class EditorGUIAssists
    {
        public static void OnGUI(object rValue)
        {

        }

        public bool IsGUIType(Type rType)
        {
            bool bIsGUIType = false;
            if (rType.IsPrimitive)
            {
                bIsGUIType = true;
            }
            else
            {
                if (rType == typeof(Vector2) || rType == typeof(Vector3) || rType == typeof(Vector4) || rType.IsEnum)
                {
                    bIsGUIType = true;
                }
            }
            return bIsGUIType;
        }
    }
}
