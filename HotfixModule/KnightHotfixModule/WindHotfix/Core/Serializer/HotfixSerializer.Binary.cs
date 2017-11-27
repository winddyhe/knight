//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.IO;
using System;

namespace WindHotfix.Core
{
    [HotfixTSIgnore]
    public class HotfixSerializerBinary
    {
        public virtual void Serialize(BinaryWriter rWriter)     { }
        public virtual void Deserialize(BinaryReader rReader)   { }
    }

    public class HotfixSerializerBinaryTypes : HotfixTypeSearchDefault<HotfixSerializerBinary> { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HotfixSBIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HotfixSBEnableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HotfixSBDynamicAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class HotfixSBGroupAttribute : Attribute
    {
        public string GroupName;

        public HotfixSBGroupAttribute(string rGroupName)
        {
            this.GroupName = rGroupName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class HotfixSBGroupIneritedAttribute : HotfixSBGroupAttribute
    {
        public HotfixSBGroupIneritedAttribute(string rGroupName)
            : base(rGroupName)
        {
        }
    }
}