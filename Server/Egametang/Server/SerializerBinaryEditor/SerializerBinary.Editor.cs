//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Core;
using System.Reflection;

namespace Core.Serializer.Editor
{
    public class SerializerBinaryEditor : TSingleton<SerializerBinaryEditor>
    {
        private const string    mGeneratePathRoot       = "../Model/Generate/SerializerBinary";
        private const string    mGeneratePath           = mGeneratePathRoot + "/";
        private const string    mCommonSerializerPath   = mGeneratePath     + "CommonSerializer.cs";

        private CodeGenerator_CommonSerializer          mCommonSerializer;
        private List<CodeGenerator_ClassSerializer>     mClassSerializers;

        private SerializerBinaryEditor()
        {
        }
        
        public static void CodeGenerate()
        {
            SerializerBinaryEditor.Instance.Analysis((rText, fProgress) => Console.WriteLine($"AutoCSGenerate: {rText}, {fProgress} ..."));
            Console.WriteLine("Auto cs generate complete..");
        }

        public void Analysis(System.Action<string, float> rProgressAction = null)
        {
            mClassSerializers = new List<CodeGenerator_ClassSerializer>();

            mCommonSerializer = new CodeGenerator_CommonSerializer(mCommonSerializerPath);
            mCommonSerializer.WriteHead();

            var rSBTypes = SerializerBinaryTypes.Types;
            for (int i = 0; i < rSBTypes.Count; i++)
            {
                var rGroupName = string.Empty;
                var rAttributes = rSBTypes[i].GetCustomAttributes<SBGroupAttribute>(true);
                if (rAttributes.Length > 0)
                    rGroupName = rAttributes[0].GroupName;
                var rClassSerializer = new CodeGenerator_ClassSerializer(UtilTool.PathCombine(mGeneratePath, rGroupName, rSBTypes[i].FullName + ".Binary.cs"));
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
