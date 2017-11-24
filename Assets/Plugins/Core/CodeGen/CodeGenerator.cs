//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Text;
using System.IO;

namespace Core
{
    public class CodeGenerator
    {
        public StringBuilder    StringBuilder;
        public string           FilePath;

        public CodeGenerator(string rFilePath)
        {
            this.FilePath = rFilePath;
            this.StringBuilder = new StringBuilder();
        }
        
        public void Write(string rContent)
        {
            this.StringBuilder?.AppendLine(rContent);
        }

        public void Write()
        {
            this.StringBuilder?.AppendLine();
        }

        public void Write(string rContent, params object[] rArgs)
        {
            this.StringBuilder?.AppendLine(string.Format(rContent, rArgs));
        }

        public void Save()
        {
            File.WriteAllText(this.FilePath, this.StringBuilder.ToString());
        }

        public virtual void WriteHead()
        {
        }

        public virtual void WriteEnd()
        {
        }
    }
}