//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Core;
using Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Framework.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(Transform), true)]
    class TransformInspector : UnityEditor.Editor
    {
        public static TransformInspector Instance;
    
        SerializedProperty  mPos;
        SerializedProperty  mRot;
        SerializedProperty  mScale;
    
        void OnEnable()
        {
            Instance = this;
            mPos     = serializedObject.FindProperty("m_LocalPosition");
            mRot     = serializedObject.FindProperty("m_LocalRotation");
            mScale   = serializedObject.FindProperty("m_LocalScale"); 
        }
    
        void OnDestroy()
        {
            Instance = null;
        }
    
        public override void OnInspectorGUI()
        {
            EditorGUIUtility.labelWidth = 15;
            serializedObject.Update();
    
            DrawPosition();
            DrawRotation();
            DrawScaling();
    
            serializedObject.ApplyModifiedProperties();
        }
    
        void DrawPosition()
        {
            bool reset = false;
            using (var space = new EditorGUILayout.HorizontalScope())
            {
                reset = GUILayout.Button("P", GUILayout.Width(20f));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(mPos.FindPropertyRelative("z"));
            }
            if (reset) mPos.vector3Value = Vector3.zero;
        }
        
        void DrawScaling()
        {
            bool reset = false;
            using(var space = new EditorGUILayout.HorizontalScope())
            {
                reset = GUILayout.Button("S", GUILayout.Width(20f));
                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("x"));
                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("y"));
                EditorGUILayout.PropertyField(mScale.FindPropertyRelative("z"));
            }
            if (reset) mScale.vector3Value = Vector3.one;
        }
    
        void DrawRotation()
        {
            bool reset = false;
            using(var space = new EditorGUILayout.HorizontalScope())
            {
                reset = GUILayout.Button("R", GUILayout.Width(20f));
    
                Vector3 visible = (serializedObject.targetObject as Transform).localEulerAngles;
    
                visible.x = UtilTool.WrapAngle(visible.x);
                visible.y = UtilTool.WrapAngle(visible.y);
                visible.z = UtilTool.WrapAngle(visible.z);
    
                Axes changed = CheckDifference(mRot);
                Axes altered = Axes.None;
                GUILayoutOption opt = GUILayout.MinWidth(30f);
    
                if (FloatField("X", ref visible.x, (changed & Axes.X) != 0, false, opt)) altered |= Axes.X;
                if (FloatField("Y", ref visible.y, (changed & Axes.Y) != 0, false, opt)) altered |= Axes.Y;
                if (FloatField("Z", ref visible.z, (changed & Axes.Z) != 0, false, opt)) altered |= Axes.Z;
    
                if (reset)
                {
                    mRot.quaternionValue = Quaternion.identity;
                }
                else if (altered != Axes.None)
                {
                    EditorAssists.RegisterUndo("Change Rotation", serializedObject.targetObjects);
                    foreach (var obj in serializedObject.targetObjects)
                    {
                        Transform t = obj as Transform;
                        Vector3 v = t.localEulerAngles;
    
                        if ((altered & Axes.X) != 0) v.x = visible.x;
                        if ((altered & Axes.Y) != 0) v.y = visible.y;
                        if ((altered & Axes.Z) != 0) v.z = visible.z;
    
                        t.localEulerAngles = v;
                    }
                }
            }
        }
    
        enum Axes : int
        {
            None = 0,
            X = 1,
            Y = 2,
            Z = 4,
            ALL = 7
        }
    
        Axes CheckDifference(Transform t, Vector3 original)
        {
            Vector3 next = t.localEulerAngles;
            Axes axes = Axes.None;
    
            if (Differs(next.x, original.x)) axes |= Axes.X;
            if (Differs(next.y, original.y)) axes |= Axes.Y;
            if (Differs(next.z, original.z)) axes |= Axes.Z;
    
            return axes;
        }
    
        Axes CheckDifference(SerializedProperty property)
        {
            Axes axes = Axes.None;
            if (property.hasMultipleDifferentValues)
            {
                Vector3 original = property.quaternionValue.eulerAngles;
                foreach (Object obj in serializedObject.targetObjects) 
                {
                    axes |= CheckDifference(obj as Transform, original);
                    if (axes == Axes.ALL) break;
                }
            }
            return axes;
        }
    
        static bool FloatField(string name, ref float value, bool hidden, bool greyedOut, GUILayoutOption opt)
        {
            float newValue = value;
            GUI.changed = false;
    
            if (!hidden)
            {
                if (greyedOut) 
                {
                    GUI.color = new Color(0.7f, 0.7f, 0.7f);
                    newValue = EditorGUILayout.FloatField(name, newValue, opt);
                    GUI.color = Color.white;
                }
                else
                {
                    newValue = EditorGUILayout.FloatField(name, newValue, opt);
                }
            }
            else if (greyedOut)
            {
                GUI.color = new Color(0.7f, 0.7f, 0.7f);
                float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
                GUI.color = Color.white;
            }
            else
            {
                float.TryParse(EditorGUILayout.TextField(name, "--", opt), out newValue);
            }
    
            if (GUI.changed && Differs(newValue, value))
            {
                value = newValue;
                return true;
            }
            return false;
        }
    
        static bool Differs(float a, float b) { return Mathf.Abs(a - b) > 0.0001f; }
    }
}