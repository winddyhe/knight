//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Knight.Core;

namespace Knight.Framework.Hotfix.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(HotfixMBContainer), true)]
    public class HotfixMBContainerInspector : UnityEditor.Editor
    {
        public class ObjectType
        {
            public SerializedProperty   Object;
            public SerializedProperty   Type;
            public SerializedProperty   Name;
            public int                  Selected;
        }

        private SerializedProperty      mHotfixName;
        private SerializedProperty      mNeedUpdate;
        private SerializedProperty      mObjects;

        private List<ObjectType>        mObjectTypes;

        void OnEnable()
        {
            this.mHotfixName    = this.serializedObject.FindProperty("mHotfixName");
            this.mNeedUpdate    = this.serializedObject.FindProperty("mNeedUpdate");
            this.mObjects       = this.serializedObject.FindProperty("mObjects");

            this.mObjectTypes   = this.ToObjectTypes(this.mObjects);
        }

        void OnDestroy()
        {
        }

        protected void DrawBaseInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            if (this.mHotfixName == null) return;

            using (var space = new EditorGUILayout.VerticalScope())
            {
                EditorGUILayout.PropertyField(this.mHotfixName, new GUIContent("Hotfix Class Name: "));
                EditorGUILayout.PropertyField(this.mNeedUpdate, new GUIContent("Hotfix Need Update: "));
                
                this.DrawUnityEngineObjects();
            }

            this.serializedObject.ApplyModifiedProperties();
        }

        private void DrawUnityEngineObjects()
        {
            this.mObjectTypes = this.ToObjectTypes(this.mObjects);
            EditorGUILayout.LabelField("Objects: ");
            for (int i = 0; i < this.mObjectTypes.Count; i++)
            {
                using (var space1 = new EditorGUILayout.HorizontalScope("TextField"))
                {
                    GUILayout.Label(i.ToString() + ": ", GUILayout.Width(15));

                    var rElementNameProperty = this.mObjectTypes[i].Name;
                    var rElementObjProperty = this.mObjectTypes[i].Object;
                    var rElementTypeProperty = this.mObjectTypes[i].Type;
                    EditorGUILayout.PropertyField(rElementNameProperty, new GUIContent(""));
                    EditorGUILayout.PropertyField(rElementObjProperty, new GUIContent(""));

                    List<string> rElemTypes = this.GetObjectComponentTypes(rElementObjProperty);
                    this.mObjectTypes[i].Selected = EditorGUILayout.Popup(this.mObjectTypes[i].Selected, rElemTypes.ToArray());
                    this.ChangeElementObjectBySelectedType(rElementObjProperty, rElementTypeProperty, this.mObjectTypes[i].Selected);

                    if (GUILayout.Button("Del", GUILayout.Width(40)))
                    {
                        this.mObjects.DeleteArrayElementAtIndex(i);
                        break;
                    }
                }
            }

            if (GUILayout.Button("Add UnityEngine Objects"))
            {
                this.mObjects.InsertArrayElementAtIndex(this.mObjects.arraySize);
                this.mObjectTypes = this.ToObjectTypes(this.mObjects);
            }
        }

        private void ChangeElementObjectBySelectedType(SerializedProperty rObjectProp, SerializedProperty rTypeProp, int nSelected)
        {
            List<string> rElemTypes = GetObjectComponentTypes(rObjectProp);
            List<UnityEngine.Object> rElemObjs = GetObjectComponents(rObjectProp);

            if (nSelected >= 0 && nSelected < rElemTypes.Count)
            {
                string rRealType = rElemTypes[nSelected];
                rTypeProp.stringValue = rRealType;
                rObjectProp.objectReferenceValue = rElemObjs[nSelected];
                return;
            }
        }

        private List<ObjectType> ToObjectTypes(SerializedProperty rObjects)
        {
            var rObjectTypes = new List<ObjectType>();
            if (rObjects == null) return rObjectTypes;

            for (int i = 0; i < rObjects.arraySize; i++)
            {
                SerializedProperty rElementProp = rObjects.GetArrayElementAtIndex(i);

                var rObjectType = new ObjectType()
                {
                    Object = rElementProp.FindPropertyRelative("Object"),
                    Type = rElementProp.FindPropertyRelative("Type"),
                    Name = rElementProp.FindPropertyRelative("Name"),
                    Selected = GetSelectedTypeIndex(rElementProp.FindPropertyRelative("Object"), rElementProp.FindPropertyRelative("Type")),
                };
                if (rObjectType.Object != null && rObjectType.Object.objectReferenceValue != null && string.IsNullOrEmpty(rObjectType.Name.stringValue))
                    rObjectType.Name.stringValue = rObjectType.Object.objectReferenceValue.name;
                rObjectTypes.Add(rObjectType);
            }
            return rObjectTypes;
        }

        private int GetSelectedTypeIndex(SerializedProperty rObjectProp, SerializedProperty rTypeProp)
        {
            string rTypeStr = rTypeProp.stringValue;
            List<string> rElemTypes = GetObjectComponentTypes(rObjectProp);
            int nFindIndex = rElemTypes.FindIndex((rItem) => { return rItem.Equals(rTypeStr); });
            if (nFindIndex == -1) return 0;

            return nFindIndex;
        }

        private List<UnityEngine.Object> GetObjectComponents(SerializedProperty rObjectProp)
        {
            if (rObjectProp == null) return new List<UnityEngine.Object>();

            GameObject rElementGo = rObjectProp.objectReferenceValue as GameObject;
            if (rElementGo == null)
            {
                var rTempCmp = rObjectProp.objectReferenceValue as Component;
                if (rTempCmp != null)
                    rElementGo = rTempCmp.gameObject;
            }
            List<UnityEngine.Object> rElemObjs = new List<UnityEngine.Object>();
            if (rElementGo != null)
            {
                rElemObjs.Add(rElementGo);
                var rElemCmps = rElementGo.GetComponents<Component>();
                for (int k = 0; k < rElemCmps.Length; k++)
                {
                    rElemObjs.Add(rElemCmps[k]);
                }
            }
            return rElemObjs;
        }

        private List<string> GetObjectComponentTypes(SerializedProperty rObjectProp)
        {
            if (rObjectProp == null) return new List<string>();

            GameObject rElementGo = rObjectProp.objectReferenceValue as GameObject;
            if (rElementGo == null)
            {
                var rTempCmp = rObjectProp.objectReferenceValue as Component;
                if (rTempCmp != null)
                    rElementGo = rTempCmp.gameObject;
            }
            List<string> rElemTypes = new List<string>();
            if (rElementGo != null)
            {
                rElemTypes.Add("UnityEngine.GameObject");
                var rElemCmps = rElementGo.GetComponents<Component>();
                for (int k = 0; k < rElemCmps.Length; k++)
                {
                    rElemTypes.Add(rElemCmps[k].GetType().ToString());
                }
            }
            return rElemTypes;
        }
    }
}
