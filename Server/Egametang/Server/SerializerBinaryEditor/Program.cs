//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace Core.Serializer.Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            DllHelper.ReferthisDLL();
            SerializerBinaryEditor.CodeGenerate();
        }
    }
}
