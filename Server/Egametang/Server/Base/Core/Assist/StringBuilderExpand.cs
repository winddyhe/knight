//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Text;

namespace Core
{
    public static class StringBuilderExpand
    {
        public static StringBuilder F(this StringBuilder rSelf, string format, params object[] param)
        {
            return rSelf.AppendFormat(format, param);
        }

        public static StringBuilder A(this StringBuilder rSelf, string text)
        {
            return rSelf.Append(text);
        }

        public static StringBuilder T(this StringBuilder rSelf, int nCount)
        {
            for (int nIndex = 0; nIndex < nCount; ++nIndex)
                rSelf.Append('\t');
            return rSelf;
        }

        public static StringBuilder S(this StringBuilder rSelf, int nCount)
        {
            for (int nIndex = 0; nIndex < nCount; ++nIndex)
                rSelf.Append(' ');
            return rSelf;
        }

        public static StringBuilder L(this StringBuilder rSelf, int nCount)
        {
            for (int nIndex = 0; nIndex < nCount; ++nIndex)
                rSelf.AppendLine();
            return rSelf;
        }

        public static StringBuilder N(this StringBuilder rSelf)
        {
            return rSelf.AppendLine();
        }
    }
}
