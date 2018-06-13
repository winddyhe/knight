//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;

namespace Knight.Core.Serializer.Editor
{
    public class SerializerBinaryEditor : TSingleton<SerializerBinaryEditor>
    {
        private CodeGenerator_CommonSerializer          mCommonSerializer;
        private List<CodeGenerator_ClassSerializer>     mClassSerializers;

        private SerializerBinaryEditor()
        {
        }

        [MenuItem("Tools/Serializer/Auto CS Generate...")]
        public static void CodeGenerate()
        {
            string rGeneratePathRoot = "Assets/Framework/Generate/SerializerBinary/";
            SerializerBinaryEditor.Instance.Analysis(rGeneratePathRoot, "Framework", (rText, fProgress) => EditorUtility.DisplayProgressBar("AutoCSGenerate", rText, fProgress));

            rGeneratePathRoot = "Assets/Game/Script/Generate/SerializerBinary/";
            SerializerBinaryEditor.Instance.Analysis(rGeneratePathRoot, "Game", (rText, fProgress)=> EditorUtility.DisplayProgressBar("AutoCSGenerate", rText, fProgress));
            EditorUtility.ClearProgressBar();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEngine.Debug.Log("Auto serializer cs generate complete..");
        }

        public void Analysis(string rGeneratePathRoot, string rDLLModuleName, System.Action<string, float> rProgressAction = null)
        {
            string rGeneratePath = rGeneratePathRoot;
            string rCommonSerializerPath = rGeneratePath + "CommonSerializer.cs";

            mClassSerializers = new List<CodeGenerator_ClassSerializer>();

            mCommonSerializer = new CodeGenerator_CommonSerializer(rCommonSerializerPath);
            mCommonSerializer.WriteHead();

            var rSBTypes = SerializerBinaryTypes.Types;
            for (int i = 0; i < rSBTypes.Count; i++)
            {
                if (!rSBTypes[i].Assembly.GetName().Name.Equals(rDLLModuleName)) continue;

                var rGroupName = string.Empty;
                var rAttributes = rSBTypes[i].GetCustomAttributes<SBGroupAttribute>(true);
                if (rAttributes.Length > 0)
                    rGroupName = rAttributes[0].GroupName;
                var rClassSerializer = new CodeGenerator_ClassSerializer(UtilTool.PathCombine(rGeneratePath, rGroupName, rSBTypes[i].FullName + ".Binary.cs"));
                rClassSerializer.WriteHead();
                rClassSerializer.WriteClass(rSBTypes[i]);
                rClassSerializer.WriteEnd();

                mClassSerializers.Add(rClassSerializer);

                var rSerializeMemberInfo = SerializerAssists.FindSerializeMembers(rSBTypes[i]);
                foreach (var rMemberInfo in rSerializeMemberInfo)
                {
                    var bDynamic = rMemberInfo.IsDefined(typeof(SBDynamicAttribute), false);
                    if (rMemberInfo.MemberType == MemberTypes.Field)
                        mCommonSerializer.AnalyzeGenerateCommon((rMemberInfo as FieldInfo).FieldType, bDynamic);
                    else if (rMemberInfo.MemberType == MemberTypes.Property)
                        mCommonSerializer.AnalyzeGenerateCommon((rMemberInfo as PropertyInfo).PropertyType, bDynamic);
                }
                UtilTool.SafeExecute(rProgressAction, $"Generate File: {rSBTypes[i].FullName}", (float)i / (float)rSBTypes.Count);
                rClassSerializer.Save();
            }
            mCommonSerializer.WriteEnd();
            mCommonSerializer.Save();
        }
    }
}
