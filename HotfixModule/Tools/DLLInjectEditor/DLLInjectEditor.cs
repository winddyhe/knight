using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Core.Editor
{
    public class DLLInjectEditor
    {
        public static void Inject_EditorAndroidDLL_StreamingAssets_Execute()
        {
            var rDelegateActionType = DLLInject.GetDelegateDefinition(
                Application.dataPath + "/Editor/DLLInject/DelegateHelper.dll", "DelegateHelper.DelegateDef", "__StreamingAssets_Execute_Delegate__");

            string rEditorAndroidDLLPath = Application.dataPath + "/../Library/UnityAssemblies/UnityEditor.Android.Extensions.dll";
            rEditorAndroidDLLPath = Path.GetFullPath(rEditorAndroidDLLPath);
            string rInjectClassName = "UnityEditor.Android.PostProcessor.Tasks.StreamingAssets";
            string rInjectMethodName = "Execute";

            DLLInject.Inject(rEditorAndroidDLLPath, rInjectClassName, rInjectMethodName, rDelegateActionType);
        }
    }
}
