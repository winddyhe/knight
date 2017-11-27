//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Text;
using System.IO;
using Core;

namespace WindHotfix.Core.Editor
{
    public class HotfixCodeGenerator
    {
        public const string     CSMD5Path = "Library/CSGenerate/";

        public StringBuilder    StringBuilder;
        public string           FilePath;
        public string           MD5;
        public string           GUID;
        public bool             NeedReimport;

        public HotfixCodeGenerator(string rFilePath)
        {
            this.FilePath = rFilePath;
            this.StringBuilder = new StringBuilder();
        }
        
        public void Write(int nTabCount, string rContent)
        {
            for (int i = 0; i < nTabCount; i++)
            {
                this.StringBuilder?.Append("\t");
            }
            this.StringBuilder?.AppendLine(rContent);
        }

        public void WriteBraceCode(int nTabCount, string rHeadContent, string rLeftBrace, string rContent, string rRightBrace)
        {
            for (int i = 0; i < nTabCount; i++)
                this.StringBuilder?.Append("\t");
            this.StringBuilder?.AppendLine(rHeadContent);

            for (int i = 0; i < nTabCount; i++)
                this.StringBuilder?.Append("\t");
            this.StringBuilder?.AppendLine(rLeftBrace);

            for (int i = 0; i < nTabCount + 1; i++)
                this.StringBuilder?.Append("\t");
            this.StringBuilder?.AppendLine(rContent);

            for (int i = 0; i < nTabCount; i++)
                this.StringBuilder?.Append("\t");
            this.StringBuilder?.AppendLine(rRightBrace);
        }

        public string GenerateBraceCode(int nTabCount, string rHeadContent, string rLeftBrace, string rContent, string rRightBrace)
        {
            StringBuilder rStringbuilder = new StringBuilder();
            for (int i = 0; i < nTabCount; i++)
                rStringbuilder.Append("\t");
            rStringbuilder.AppendLine(rHeadContent);

            for (int i = 0; i < nTabCount; i++)
                rStringbuilder.Append("\t");
            rStringbuilder.AppendLine(rLeftBrace);

            for (int i = 0; i < nTabCount + 1; i++)
                rStringbuilder.Append("\t");
            rStringbuilder.AppendLine(rContent);

            for (int i = 0; i < nTabCount; i++)
                rStringbuilder.Append("\t");
            rStringbuilder.AppendLine(rRightBrace);

            return rStringbuilder.ToString();
        }

        public void Write()
        {
            this.StringBuilder?.AppendLine();
        }

        public void Write(int nTabCount, string rContent, params object[] rArgs)
        {
            for (int i = 0; i < nTabCount; i++)
            {
                this.StringBuilder?.Append("\t");
            }
            this.StringBuilder?.AppendLine(string.Format(rContent, rArgs));
        }

        public void Save()
        {
            if (!Directory.Exists(CSMD5Path))
                Directory.CreateDirectory(CSMD5Path);

            UtilTool.WriteAllText(this.FilePath, this.StringBuilder.ToString());
        }

        public virtual void WriteHead()
        {
        }

        public virtual void WriteEnd()
        {
        }
    }
}