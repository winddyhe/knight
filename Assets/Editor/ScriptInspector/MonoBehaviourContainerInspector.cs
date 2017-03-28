using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Core;

namespace Framework.Hotfix.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviourContainer), true)]
    public class MonoBehaviourContainerInspector : UnityEditor.Editor
    {
        public class ObjectType
        {
            public SerializedProperty   Object;
            public SerializedProperty   Type;
            public int                  Selected;
        }

        public static MonoBehaviourContainerInspector   Instance;

        private SerializedProperty                      mHotfixName;
        private SerializedProperty                      mObjects;
        private SerializedProperty                      mTypes;
        
        private List<ObjectType>                        mObjectTypes;

        void OnEnable()
        {
            Instance = this;

            this.mHotfixName = this.serializedObject.FindProperty("mHotfixName");
            this.mObjects = this.serializedObject.FindProperty("mObjects");
            this.mTypes = this.serializedObject.FindProperty("mTypes");
            
            this.mObjectTypes = this.ToObjectTypes(this.mObjects, this.mTypes);
        }

        void OnDestroy()
        {
            Instance = null;
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
                this.mObjectTypes = this.ToObjectTypes(this.mObjects, this.mTypes);

                EditorGUILayout.PropertyField(this.mHotfixName, new GUIContent("Hotfix Class Name: "));
                EditorGUILayout.LabelField("Objects: ");
                for (int i = 0; i < this.mObjectTypes.Count; i++)
                {
                    using (var space1 = new EditorGUILayout.HorizontalScope("TextField"))
                    {
                        GUILayout.Label(i.ToString()+": ", GUILayout.Width(15));

                        var rElementObjProperty = this.mObjectTypes[i].Object;
                        var rElementTypeProperty = this.mObjectTypes[i].Type;
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

                if (GUILayout.Button("Add"))
                {
                    this.mObjects.InsertArrayElementAtIndex(this.mObjects.arraySize);
                    this.mTypes.InsertArrayElementAtIndex(this.mTypes.arraySize);
                    this.mObjectTypes = this.ToObjectTypes(this.mObjects, this.mTypes);
                }
            }

            this.serializedObject.ApplyModifiedProperties();
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

        private List<ObjectType> ToObjectTypes(SerializedProperty rObjects, SerializedProperty rTypes)
        {
            var rObjectTypes = new List<ObjectType>();
            if (rObjects == null) return rObjectTypes;

            for (int i = 0; i < rObjects.arraySize; i++)
            {
                var rObjectType = new ObjectType()
                {
                    Object = rObjects.GetArrayElementAtIndex(i),
                    Type = rTypes.GetArrayElementAtIndex(i),
                    Selected = GetSelectedTypeIndex(rObjects.GetArrayElementAtIndex(i), rTypes.GetArrayElementAtIndex(i)),
                };
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

        private List<string> GetObjectComponentTypes_Display(SerializedProperty rObjectProp)
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
                rElemTypes.Add(rElementGo.name + " (UnityEngine.GameObject)");
                var rElemCmps = rElementGo.GetComponents<Component>();
                for (int k = 0; k < rElemCmps.Length; k++)
                {
                    rElemTypes.Add(rElementGo.name + " (" + rElemCmps[k].GetType().ToString() + ")");
                }
            }
            return rElemTypes;
        }
    }
}
