using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ReorderableKeyListAttribute))]
    public class ReorderableKeyListPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> reorderableListsByPropertyName = new Dictionary<string, ReorderableList>();

        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);

            if (property.isArray)
            {
                if (!this.reorderableListsByPropertyName.ContainsKey(property.name))
                {
                    ReorderableList reorderableList = new ReorderableList(property.serializedObject, property, true, true, true, true)
                    {
                        drawHeaderCallback = (Rect rect) =>
                        {
                            EditorGUI.LabelField(rect, string.Format("{0}: {1}", property.displayName, property.arraySize), EditorStyles.label);
                        },

                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                        {
                            var element = property.GetArrayElementAtIndex(index);
                            rect.y += 2f;
                            
                            var key = "";
                            if (element != null && element.objectReferenceValue != null)
                                key = element.objectReferenceValue.GetType().GetField("Key").GetValue(element.objectReferenceValue) as string;

                            using (var scope = new EditorGUILayout.HorizontalScope())
                            {
                                EditorGUI.LabelField(new Rect(rect.x, rect.y, 30, EditorGUIUtility.singleLineHeight), "Key");
                                key = EditorGUI.TextField(new Rect(rect.x + 30, rect.y, rect.width * 0.3f, EditorGUIUtility.singleLineHeight), key);

                                if (element != null && element.objectReferenceValue != null)
                                    element.objectReferenceValue.GetType().GetField("Key").SetValue(element.objectReferenceValue, key);

                                EditorGUIUtility.labelWidth = 53;
                                EditorGUI.PropertyField(new Rect(rect.x + 30 + rect.width * 0.35f, rect.y, rect.width * 0.65f - 30, EditorGUIUtility.singleLineHeight), element);
                            }
                        }
                    };

                    this.reorderableListsByPropertyName[property.name] = reorderableList;
                }

                this.reorderableListsByPropertyName[property.name].DoLayoutList();
            }
            else
            {
                string warning = typeof(ReorderableKeyListAttribute).Name + " can be used only on arrays or lists";
                EditorGUILayout.HelpBox(warning, MessageType.Warning);
                Debug.LogWarning(warning, PropertyUtility.GetTargetObject(property));

                EditorDrawUtility.DrawPropertyField(property);
            }
        }

        public override void ClearCache()
        {
            this.reorderableListsByPropertyName.Clear();
        }
    }
}
