using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditorTypeAttribute : Attribute
    {
        public Type     Type;

        public EditorTypeAttribute(Type rType)
        {
            this.Type = rType;
        }
    }

    public class EditorGUITypes : TypeSearchDefault<EditorGUIBase>
    {
    }

    [TSIgnore]
    public class EditorGUIBase
    {
        public object   Value;

        public string   HeadText;
        public float    Width;

        public virtual void OnGUI()
        {
        }
    }

    [EditorType(typeof(float))]
    public class EditorGUIFloat : EditorGUIBase
    {
        public override void OnGUI()
        {
            float fValue = (float)this.Value;
            fValue = EditorGUILayout.FloatField(this.HeadText, fValue, GUILayout.Width(this.Width));
            this.Value = fValue;
        }
    }

    [EditorType(typeof(bool))]
    public class EditorGUIBool : EditorGUIBase
    {
        public override void OnGUI()
        {
            bool bValue = (bool)this.Value;
            bValue = EditorGUILayout.Toggle(this.HeadText, bValue, GUILayout.Width(this.Width));
            this.Value = bValue;
        }
    }

    [EditorType(typeof(Vector2))]
    public class EditorGUIVector2 : EditorGUIBase
    {
        public override void OnGUI()
        {
            Vector2 rValue = (Vector2)this.Value;
            rValue = EditorGUILayout.Vector2Field(this.HeadText, rValue, GUILayout.Width(this.Width));
            this.Value = rValue;
        }
    }

    [EditorType(typeof(Vector3))]
    public class EditorGUIVector3 : EditorGUIBase
    {
        public override void OnGUI()
        {
            Vector3 rValue = (Vector3)this.Value;
            rValue = EditorGUILayout.Vector3Field(this.HeadText, rValue, GUILayout.Width(this.Width));
            this.Value = rValue;
        }
    }

    [EditorType(typeof(Vector4))]
    public class EditorGUIVector4 : EditorGUIBase
    {
        public override void OnGUI()
        {
            Vector4 rValue = (Vector4)this.Value;
            rValue = EditorGUILayout.Vector4Field(this.HeadText, rValue, GUILayout.Width(this.Width));
            this.Value = rValue;
        }
    }

    [EditorType(typeof(string))]
    public class EditorGUIString : EditorGUIBase
    {
        public override void OnGUI()
        {
            string rValue = (string)this.Value;
            rValue = EditorGUILayout.TextField(this.HeadText, rValue, GUILayout.Width(this.Width));
            this.Value = rValue;
        }
    }

    [TSIgnore]
    public class EditorGUIObject : EditorGUIBase
    {
        private List<EditorGUIBase> mGUIControllers;
        private List<Type>          mTypes;

        public EditorGUIObject(object rObject)
        {
            this.Value = rObject;
            this.Anaysis(this.Value);
        }

        private void Anaysis(object rValue)
        {
            this.mTypes = new List<Type>();
            for (int i = 0; i < EditorGUITypes.Types.Count; i++)
            {
                Type rGUIType = EditorGUITypes.Types[i];
                var rEditorAttr = rGUIType.GetCustomAttribute<EditorTypeAttribute>(false);
                if (rEditorAttr != null)
                    this.mTypes.Add(rEditorAttr.Type);
            }

            this.mGUIControllers = new List<EditorGUIBase>();
            Type rObjType = rValue.GetType();

            var rAllFields = rObjType.GetFields(System.Reflection.BindingFlags.CreateInstance);
            for (int i = 0; i < rAllFields.Length; i++)
            {
                Type rType = rAllFields[i].FieldType;
            }
        }

        public override void OnGUI()
        {

        }
    }
}
