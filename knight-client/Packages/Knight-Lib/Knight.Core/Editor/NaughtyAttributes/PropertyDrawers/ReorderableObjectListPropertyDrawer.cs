using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Knight.Core;
using System.Collections;

namespace NaughtyAttributes.Editor
{
    [PropertyDrawer(typeof(ReorderableObjectListAttribute))]
    public class ReorderableObjectListPropertyDrawer : PropertyDrawer
    {
        private Dictionary<string, ReorderableList> reorderableListsByPropertyName = new Dictionary<string, ReorderableList>();

        public override void DrawProperty(SerializedProperty property)
        {
            EditorDrawUtility.DrawHeader(property);
            if (property.isArray)
            {
                var targetObject = property.serializedObject.targetObject;
                var propInfo = targetObject.GetType().GetField(property.name);
                var listObj = (IList)propInfo.GetValue(targetObject);
                
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
                            rect.y += 2f;
                            var elementObj = listObj[index];
                            var element = property.GetArrayElementAtIndex(index);
                            float y = rect.y;

                            var allElementFileds = elementObj.GetType().GetFields();
                            for (int i = 0; i < allElementFileds.Length; i++)
                            {
                                var elementChildProp = element.FindPropertyRelative(allElementFileds[i].Name);
                                if (elementChildProp == null) continue;
                                EditorGUI.PropertyField(new Rect(rect.x, y, rect.width, EditorGUIUtility.singleLineHeight), elementChildProp);
                                y += 20;
                                rect.y = y;
                            }
                        }
                    };
                    reorderableList.elementHeightCallback = (int index) =>
                    {
                        var element = property.GetArrayElementAtIndex(index);
                        var elementObj = listObj[index];
                        var allElementFileds = elementObj.GetType().GetFields();
                        int y = 0;
                        for (int i = 0; i < allElementFileds.Length; i++)
                        {
                            var elementChildProp = element.FindPropertyRelative(allElementFileds[i].Name);
                            if (elementChildProp == null) continue;
                            y += 22;
                        }
                        return y;
                    };
                    this.reorderableListsByPropertyName[property.name] = reorderableList;
                }
                this.reorderableListsByPropertyName[property.name].DoLayoutList();
            }
            else
            {
                string warning = typeof(ReorderableListAttribute).Name + " can be used only on arrays or lists";
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
