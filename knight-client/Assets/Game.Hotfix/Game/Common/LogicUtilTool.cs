using System;
using System.Collections.Generic;

namespace Game
{
    public static class LogicUtilTool
    {
        public static string ToCountString(int nCount)
        {
            if (nCount >= 10000)
            {
                return string.Format("{0:f2}万", (nCount / 10000.0f));
            }
            else if (nCount >= 100000000)
            {
                return string.Format("{0:f2}亿", nCount / 100000000.0f);
            }
            else
            {
                return nCount.ToString();
            }
        }
    }
}
