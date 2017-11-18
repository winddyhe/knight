//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections.Generic;
using System.IO;
using Core;
using System;
using System.Xml;

namespace Core.Editor
{
    [TSIgnore]
    public class AutoCSGenerate
    {
        public virtual void CSGenerateProcess(CSGenerate rGenerate) { }
    }
    public class AutoCSGenerateTypes : TypeSearchDefault<AutoCSGenerate> { }
    
    public static class AutoCSGenerateMain
    {
        public static void AutoCSGenerate()
        {
            var rCSGenerate = new CSGenerate();
            for (int nIndex = 0; nIndex < AutoCSGenerateTypes.Types.Count; ++ nIndex)
            {
                var rType = AutoCSGenerateTypes.Types[nIndex];
                System.Console.WriteLine(string.Format("Process {0}...", rType.FullName), (float)nIndex / (float)AutoCSGenerateTypes.Types.Count);

                var rProcessInstance = ReflectExpand.TConstruct<AutoCSGenerate>(rType);
                if (null == rProcessInstance)
                {
                    Model.Log.Error($"AutoCSGenerateMain => {rType.FullName} not use. need default construct");
                    continue;
                }

                try
                {
                    rProcessInstance.CSGenerateProcess(rCSGenerate);
                }
                catch (System.Exception rException)
                {
                    Model.Log.Error($"{rException.Message}, {rException.StackTrace}");
                }
            }
            
            rCSGenerate.UpdateCSFile( (rText, fRate) => {
                Model.Log.Error($"AutoCSGenerate {rText}, {fRate}");
            });
        }
    }

    public class CSGenerate
    {
        class CSInfo
        {
            public string Text;
            public string SavePath;
        }

        public void Add(string rText, string rSavePath)
        {
            mCSInfo.Add(new CSInfo() { Text = rText, SavePath = rSavePath });
        }

        public void AddHead(string rText, string rSavePath)
        {
            mCSInfo.Insert(0, new CSInfo() { Text = rText, SavePath = rSavePath });
        }
        public void Clear()
        {
            mCSInfo.Clear();
        }
        public List<string> AllGenerateFiles
        {
            get
            {
                var rFiles = new List<string>();
                foreach(var rCSInfo in mCSInfo)
                    rFiles.Add(rCSInfo.SavePath);
                return rFiles;
            }
        }
        public void UpdateCSFile(System.Action<string, float> rFeedback)
        {
            for (int nIndex = 0; nIndex < mCSInfo.Count; ++nIndex)
            {
                var rCSInfo = mCSInfo[nIndex];

                if (null != rFeedback)
                    rFeedback($"Update {rCSInfo.SavePath}", (float)(nIndex + 1)/(float)mCSInfo.Count);
                   
                WriteFile(rCSInfo.Text, rCSInfo.SavePath);
            }

            //this.GenCsproj();
        }

        /// <summary>
        /// 在.csproj文件中导入新文件  
        /// </summary>
        void GenCsproj()
        {
            //ProtocolModel目前的cs文件列表  
            if (!Directory.Exists(GeneratePath)) return;

            var files = Directory.GetFiles(GeneratePath, "*.cs", SearchOption.AllDirectories);

            List<String> currFiles = new List<String>();
            foreach (var file in files)
            {
                String path = file.ToString().Replace('/', '\\');
                path = path.Substring(path.IndexOf("Knight\\Generate\\SerializerBinary"));
                currFiles.Add(path);
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(CsprojFile);
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
                            //项目中已包含的ProtocolModel  
                            if (includeFile.StartsWith(@"Knight\Generate\SerializerBinary\"))
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
            doc.Save(CsprojFile);
        }
        void WriteFile(string rText, string rSavePath)
        {
            var rPath = Path.GetDirectoryName(rSavePath);
            if (!Directory.Exists(rPath))
                Directory.CreateDirectory(rPath);

            File.WriteAllText(rSavePath, rText);
        }

        public string CsprojFile;
        public string GeneratePath;

        List<CSInfo> mCSInfo = new List<CSInfo>();
    }
}
