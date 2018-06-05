//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Knight.Core.Serializer.Editor
{
    public class SerializerBinaryEditor : TSingleton<SerializerBinaryEditor>
    {
        private const string mGeneratePathRoot      = "../../../../Hotfix/Module/Message/Generate/SerializerBinary";
        private const string mGeneratePath          = mGeneratePathRoot + "/";
        private const string mCommonSerializerPath  = mGeneratePath + "CommonSerializer.cs";
        private const string mCsprojFile            = "../../../../Hotfix/Server.Hotfix.csproj";


        private CodeGenerator_CommonSerializer mCommonSerializer;
        private List<CodeGenerator_ClassSerializer> mClassSerializers;

        private SerializerBinaryEditor()
        {
        }

        public static void CodeGenerate()
        {
            SerializerBinaryEditor.Instance.Analysis((rText, fProgress) => Console.WriteLine($"AutoCSGenerate: {rText}, {fProgress}"));
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
                var rAttributes = new List<SBGroupAttribute>();
                rAttributes.AddRange(rSBTypes[i].GetCustomAttributes<SBGroupAttribute>(true));
                if (rAttributes.Count > 0)
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

            //this.GenCsproj();
        }

        /// <summary>
        /// 在.csproj文件中导入新文件  
        /// </summary>
        private void GenCsproj()
        {
            //ProtocolModel目前的cs文件列表  
            if (!Directory.Exists(mGeneratePath)) return;

            var files = Directory.GetFiles(mGeneratePath, "*.cs", SearchOption.AllDirectories);

            List<String> currFiles = new List<String>();
            foreach (var file in files)
            {
                String path = file.ToString().Replace('/', '\\');
                path = path.Substring(path.IndexOf("Module\\Message\\Generate\\SerializerBinary"));
                currFiles.Add(path);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(mCsprojFile);
            //Project节点  
            XmlNodeList xnl = doc.ChildNodes[0].ChildNodes;
            if (doc.ChildNodes[0].Name.ToLower() != "project")
            {
                xnl = doc.ChildNodes[1].ChildNodes;
            }
            foreach (XmlNode xn in xnl)
            {
                //找到包含compile的节点  
                if (xn.ChildNodes.Count > 0 && xn.ChildNodes[0].Name.ToLower() == "compile")
                {
                    foreach (XmlNode cxn in xn.ChildNodes)
                    {
                        if (cxn.Name.ToLower() == "compile")
                        {
                            XmlElement xe = (XmlElement)cxn;
                            String includeFile = xe.GetAttribute("Include");

                            if (includeFile.StartsWith(@"Module\\Message\Generate\SerializerBinary\"))
                            {
                                Console.WriteLine(includeFile);
                                foreach (String item in currFiles)
                                {
                                    //将已经包含在项目中的cs文件在所有文件的列表中剔除  
                                    //操作完之后currFiles中剩下的就是接下来需要包含到项目中的文件  
                                    if (item.Equals(includeFile))
                                    {
                                        currFiles.Remove(item);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    //将剩下的文件加入csproj中  
                    foreach (String item in currFiles)
                    {
                        XmlElement xelKey = doc.CreateElement("Compile", doc.DocumentElement.NamespaceURI);
                        XmlAttribute xelType = doc.CreateAttribute("Include");
                        xelType.InnerText = item;
                        xelKey.SetAttributeNode(xelType);
                        xn.AppendChild(xelKey);
                    }
                }
            }
            doc.Save(mCsprojFile);
        }
    }
}