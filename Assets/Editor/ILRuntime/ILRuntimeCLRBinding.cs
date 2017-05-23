#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using Core;
using Core.WindJson;
using Framework.Hotfix;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    
    [MenuItem("Tools/ILRuntime/Generate CLR Binding Code", priority = 550)]
    static void GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();
        types.Add(typeof(int));
        types.Add(typeof(float));
        types.Add(typeof(long));
        types.Add(typeof(object));
        types.Add(typeof(string));
        types.Add(typeof(Array));
        types.Add(typeof(Vector2));
        types.Add(typeof(Vector3));
        types.Add(typeof(Vector4));
        types.Add(typeof(Color));
        types.Add(typeof(MonoBehaviour));
        types.Add(typeof(Quaternion));
        types.Add(typeof(GameObject));
        types.Add(typeof(UnityEngine.Object));
        types.Add(typeof(Transform));
        types.Add(typeof(RectTransform));
        types.Add(typeof(Time));
        types.Add(typeof(UnityEngine.Debug));
        //所有DLL内的类型的真实C#类型都是ILTypeInstance
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(Dictionary<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(Dict<ILRuntime.Runtime.Intepreter.ILTypeInstance, ILRuntime.Runtime.Intepreter.ILTypeInstance>));
        types.Add(typeof(Dictionary<object, object>));
        types.Add(typeof(Dict<object, object>));
        types.Add(typeof(JsonParser));
        types.Add(typeof(JsonArray));
        types.Add(typeof(JsonClass));
        types.Add(typeof(JsonData));
        types.Add(typeof(HotfixEventHandler));

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/ILRuntime/Generated");

        AssetDatabase.Refresh();
    }
}
#endif
