//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Core;

namespace Game.Knight
{
    public class GamePlayComponentParser
    {
        public string originData;

        public GamePlayComponentParser(string rOriginData)
        {
            this.originData = rOriginData;
        }

        public List<GPCSymbolObject> Parser()
        {
            int end = 0;
            int i = 0;
            GPCSymbolItem rCurSymbol = null;
            GPCSymbolElement rCurElem = null;
            GPCSymbolObject rCurSymbolObj = null;

            List<GPCSymbolObject> rSymbolObjs = new List<GPCSymbolObject>();
            Stack<GPCSymbolItem> rNodeStack = new Stack<GPCSymbolItem>();
            while(i < this.originData.Length)
            {
                if (!string.IsNullOrEmpty(LexicalAnalysis.isSpecialSymbol(originData, i, ref end)) || !string.IsNullOrEmpty(LexicalAnalysis.isComment(originData, i, ref end)))
                {
                    i = end;
                    continue;
                }
                rCurSymbol = buildSymbolItem(rCurSymbol, i, ref end);
                if (rCurSymbol != null)
                {
                    switch (rCurSymbol.Type)
                    {
                        case GPCSymbolType.ObjStart:
                            rCurSymbolObj = new GPCSymbolObject();
                            rCurSymbolObj.Head = new GPCSymbolElement(rCurElem.Identifer, rCurElem.Args);
                            rCurSymbolObj.Bodies = new List<GPCSymbolElement>();
                            break;
                        case GPCSymbolType.ObjEnd:
                            rSymbolObjs.Add(rCurSymbolObj);
                            rCurSymbolObj = null;
                            break;
                        case GPCSymbolType.ArgsStart:
                            GPCSymbolItem rPeekNode = rNodeStack.Pop();
                            rCurElem = new GPCSymbolElement();
                            rCurElem.Identifer = rPeekNode;
                            rCurElem.Args = new List<GPCSymbolItem>();
                            break;
                        case GPCSymbolType.ArgsEnd:
                            GPCSymbolItem rPeekNode1 = rNodeStack.Pop();
                            rCurElem.Args.Add(rPeekNode1);
                            break;
                        case GPCSymbolType.ArgsSplit:
                            GPCSymbolItem rPeekNode2 = rNodeStack.Pop();
                            rCurElem.Args.Add(rPeekNode2);
                            break;
                        case GPCSymbolType.ElementSplit:
                            rCurSymbolObj.Bodies.Add(rCurElem);
                            rCurElem = null;
                            break;
                        case GPCSymbolType.Identifer:
                            rNodeStack.Push(rCurSymbol);
                            break;
                        case GPCSymbolType.Arg:
                            rNodeStack.Push(rCurSymbol);
                            break;
                        default:
                            break;
                    }
                    i = end;
                    continue;
                }
                i++;
            }
            return rSymbolObjs;
        }

        private GPCSymbolItem buildSymbolItem(GPCSymbolItem rCurSymbol, int begin, ref int end)
        {
            if (originData[begin] == '{')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ObjStart, Value = "{" };
            }
            else if (originData[begin] == '}')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ObjEnd, Value = "}" };
            }
            else if (originData[begin] == '(')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ArgsStart, Value = "(" };
            }
            else if (originData[begin] == ')')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ArgsEnd, Value = ")" };
            }
            else if (originData[begin] == ',')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ArgsSplit, Value = "," };
            }
            else if (originData[begin] == ';')
            {
                end = begin + 1;
                return new GPCSymbolItem() { Type = GPCSymbolType.ElementSplit, Value = ";" };
            }

            string tempWord = "";
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.IsNotKeywordIdentifer(originData, begin, ref end)))
            {
                return new GPCSymbolItem() { Type = GPCSymbolType.Identifer, Value = tempWord };
            }
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isString(originData, begin, ref end)))
            {
                tempWord = tempWord.Substring(1, tempWord.Length - 2);
                return new GPCSymbolItem() { Type = GPCSymbolType.Arg, Value = tempWord };
            }
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isKeyword(originData, begin, ref end)))
            {
                return new GPCSymbolItem() { Type = GPCSymbolType.Arg, Value = tempWord };
            }
            if (!string.IsNullOrEmpty(tempWord = LexicalAnalysis.isDigit(originData, begin, ref end)))
            {
                return new GPCSymbolItem() { Type = GPCSymbolType.Arg, Value = tempWord };
            }
            return null;
        }
    }
}

