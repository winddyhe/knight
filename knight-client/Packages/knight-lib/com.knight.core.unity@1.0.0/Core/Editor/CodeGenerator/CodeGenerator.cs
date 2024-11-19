using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace Knight.Core.Editor
{
    public class CodeGenerator
    {
        public const string CSMD5Path = "Library/CSGenerate/";

        public StringBuilder StringBuilder;
        public string FilePath;
        public string MD5;
        public string GUID;
        public bool NeedReimport;
        public Type ClassType;      // 类类型

        public CodeGenerator(string rFilePath)
        {
            this.FilePath = rFilePath;
            this.StringBuilder = new StringBuilder();
            this.GUID = Guid.NewGuid().ToString();
        }

        public virtual void CodeGenerate()
        {
        }

        public void Save()
        {
            if (!Directory.Exists(CSMD5Path))
                Directory.CreateDirectory(CSMD5Path);

            this.NeedReimport = false;

            if (string.IsNullOrEmpty(this.GUID) ||
                !File.Exists(CSMD5Path + this.GUID) ||
                !File.Exists(this.FilePath) ||
                File.ReadAllText(CSMD5Path + this.GUID) != this.MD5)
            {
                this.NeedReimport = true;
                PathTool.WriteFile(this.FilePath, this.StringBuilder.ToString());
                this.MD5 = MD5Tool.GetStringMD5(this.StringBuilder.ToString()).ToHEXString();
                PathTool.WriteFile(CSMD5Path + this.GUID, this.MD5);
            }
        }
    }
}
