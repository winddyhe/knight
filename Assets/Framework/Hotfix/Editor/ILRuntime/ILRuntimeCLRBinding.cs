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
        types.Add(typeof(Color));
        types.Add(typeof(MonoBehaviour));
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
        types.Add(typeof(HotfixEventManager));
        types.Add(typeof(Delegate));

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/Generate/ILRuntime");

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream("Assets/Game/Knight/GameAsset/Hotfix/Libs/KnightHotfixModule.bytes", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
        }
        //Crossbind Adapter is needed to generate the correct binding code
        InitILRuntime(domain);
        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Generate/ILRuntime");
    }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor(new CoroutineAdaptor());
        domain.RegisterCrossBindingAdaptor(new IEqualityComparerAdaptor());
        domain.RegisterCrossBindingAdaptor(new IEnumerableAdaptor());
        domain.RegisterCrossBindingAdaptor(new IAsyncStateMachineAdaptor());
    }
}
#endif
