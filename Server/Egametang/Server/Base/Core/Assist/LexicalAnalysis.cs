//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    /// <summary>
    /// 词法分析数据：转换矩阵和数据结构
    /// </summary>
    public class LexicalAnalysis
    {
        /// <summary>
        /// 注释的词法状态转换矩阵
        /// a -> '/'    a -> '*'    a -> 'other'    a -> '\n'
        ///    0           1             2              3
        /// 5表示终态是注释，6表示出错了不是注释
        /// </summary>
        public static int[][] CommentsStateMatrix = new int[][]
        {
            new int[] { 1, 6, 6, 6 },   //0
            new int[] { 4, 2, 6, 6 },   //1
            new int[] { 2, 3, 2, 2 },   //2
            new int[] { 5, 2, 2, 2 },   //3
            new int[] { 4, 4, 4, 5 },   //4
        };

        /// <summary>
        /// 特殊字符的词法状态转化矩阵
        /// a -> '\b' '\f' '\r' '\t' '\n' ' '   a -> other
        /// 2表示正常结束态，3表示出错结束态
        /// </summary>
        public static int[][] SpecialSymbolMatrix = new int[][]
        {
            new int[] { 1, 3 }, //0
            new int[] { 1, 2 }  //1
        };

        /// <summary>
        /// 字符串的词法状态转换矩阵
        /// a -> '"'   a -> other
        /// 3正常结束态，4表示错误结束态
        /// </summary>
        public static int[][] StringMatrix = new int[][]
        {
            new int[] { 1, 5 }, //0
            new int[] { 3, 2 }, //1
            new int[] { 3, 2 }, //2
            new int[] { 4, 4 }  //3
        };

        /// <summary>
        /// 标示符的词法状态转换矩阵
        /// a -> letter   a -> digit  a -> _  a -> other
        ///     0             1          2       3
        /// 2表示正常结束态，3表示错误结束态
        /// </summary>
        public static int[][] IdentiferMatrix = new int[][]
        {
            new int[] { 1, 3, 1, 3 },
            new int[] { 1, 1, 1, 2 }
        };

        /// <summary>
        /// json中的关键字 false true null
        /// </summary>
        public static List<string> Keywords = new List<string>()
        {
            "false",
            "true",
            "null"
        };

        /// <summary>
        /// 数字的词法状态转换矩阵
        /// a -> digit(0)   a -> digit(1-9)  a -> E/e   a -> .   a -> +     a -> -    a -> other
        ///    0                    1           2         3         4          5         6
        /// 9表示正常结束态、10表示错误结束态
        /// </summary>
        public static int[][] DigitMatrix = new int[][]
        {
            new int[] { 2, 1, 10, 4,  10, 3,  10 },   //0
            new int[] { 1, 1, 6,  4,  9,  9,  9  },   //1
            new int[] { 1, 1, 6,  4,  9,  9,  9  },   //2
            new int[] { 2, 1, 10, 4,  10, 10, 10 },   //3
            new int[] { 5, 5, 6,  9,  9,  9,  9  },   //4
            new int[] { 5, 5, 6,  9,  9,  9,  9  },   //5
            new int[] { 8, 8, 10, 10, 7,  10, 10 },   //6
            new int[] { 8, 8, 10, 10, 10, 10, 10 },   //7
            new int[] { 8, 8, 9,  9,  9,  9,  9  },   //8
        };

        private static int isCommitInputChar(char c)
        {
            int j = 0;
            if (c == '/') j = 0;
            else if (c == '*') j = 1;
            else if (c == '\n') j = 3;
            else j = 2;        //other
            return j;
        }

        /// <summary>
        /// 是否是注释 /**/ 和 //
        /// </summary>
        public static string isComment(string originData, int begin, ref int end)
        {
            string result = checkLexical(originData, isCommitInputChar, LexicalAnalysis.CommentsStateMatrix, 5, 6, begin, ref end);
            if (!string.IsNullOrEmpty(result)) result += originData[end++];
            return result;
        }

        private static int isSpecialSymbolInputChar(char c)
        {
            int j = 0;
            if (c == '\b' || c == '\f' || c == '\r' || c == '\n' || c == ' ' || c == '\0' || c == '\t')
                j = 0;
            else
                j = 1;
            return j;
        }

        /// <summary>
        /// 是否是特殊的字符 \b \f \n \r \t 空格
        /// </summary>
        public static string isSpecialSymbol(string originData, int begin, ref int end)
        {
            return checkLexical(originData, isSpecialSymbolInputChar, LexicalAnalysis.SpecialSymbolMatrix, 2, 3, begin, ref end);
        }

        private static int isStringInputChar(char c)
        {
            int j = 0;
            if (c == '"') j = 0;
            else j = 1;
            return j;
        }

        /// <summary>
        /// 是否为字符串 "string"
        /// </summary>
        public static string isString(string originData, int begin, ref int end)
        {
            return checkLexical(originData, isStringInputChar, LexicalAnalysis.StringMatrix, 4, 5, begin, ref end);
        }

        private static int isIdentifierInputChar(char c)
        {
            int j = 0;
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
                j = 0;
            else if (c >= '0' && c <= '9')
                j = 1;
            else if (c == '_')
                j = 2;
            else
                j = 3;
            return j;
        }

        /// <summary>
        /// 是否是标识符
        /// </summary>
        public static string isIdentifier(string originData, int begin, ref int end)
        {
            return checkLexical(originData, isIdentifierInputChar, LexicalAnalysis.IdentiferMatrix, 2, 3, begin, ref end);
        }

        /// <summary>
        /// 是否为关键字 false true null
        /// </summary>
        public static string isKeyword(string originData, int begin, ref int end)
        {
            string tempWord = isIdentifier(originData, begin, ref end);
            if (LexicalAnalysis.Keywords.Contains(tempWord))
                return tempWord;
            else
                return "";
        }

        /// <summary>
        /// 是否为不是关键字的标识符
        /// </summary>
        public static string IsNotKeywordIdentifer(string originData, int begin, ref int end)
        {
            string tempWord = isIdentifier(originData, begin, ref end);
            if (!LexicalAnalysis.Keywords.Contains(tempWord))
                return tempWord;
            else
                return "";
        }

        private static int isDigitInputChar(char c)
        {
            int j = 0;
            if (c == '0')
                j = 0;
            else if (c >= '1' && c <= '9')
                j = 1;
            else if (c == 'E' || c == 'e')
                j = 2;
            else if (c == '.')
                j = 3;
            else if (c == '+')
                j = 4;
            else if (c == '-')
                j = 5;
            else
                j = 6;
            return j;
        }

        /// <summary>
        /// 是否为数字，包括整数、实数、科学计数
        /// </summary>
        public static string isDigit(string originData, int begin, ref int end)
        {
            return checkLexical(originData, isDigitInputChar, LexicalAnalysis.DigitMatrix, 9, 10, begin, ref end);
        }

        /// <summary>
        /// 根据词法分析状态机来分析Json字符串中的每种单词的类型
        /// </summary>
        private static string checkLexical(string originData, System.Func<char, int> isInputCharFunc, int[][] stateMatrix,
                                           int lastState, int errorState, int begin, ref int end)
        {
            string temp = "";
            int pCurrent = begin;
            int nState = 0;         //初始状态为0
            int nInputChar = 0;

            while (pCurrent < originData.Length)
            {
                nInputChar = isInputCharFunc(originData[pCurrent]);
                if (nState != lastState && nState != errorState)
                {
                    nState = stateMatrix[nState][nInputChar];
                }

                if (nState != lastState && nState != errorState)
                    temp += originData[pCurrent];

                if (nState == lastState)
                {
                    end = pCurrent;
                    break;
                }
                if (nState == errorState)
                {
                    end = pCurrent;
                    temp = "";
                    break;
                }
                pCurrent++;
            }

            if (nState != lastState) temp = "";

            return temp;
        }
    }
}
