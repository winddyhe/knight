//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.IO;
using System;
using Core;

namespace Core.Serializer
{
    [TSIgnore]
    public class SerializerBinary
    {
        public virtual void Serialize(BinaryWriter rWriter)     { }
        public virtual void Deserialize(BinaryReader rReader)   { }
    }

    public class SerializerBinaryTypes : TypeSearchDefault<SerializerBinary> { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBEnableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SBDynamicAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SBGroupAttribute : Attribute
    {
        public string GroupName;

        public SBGroupAttribute(string rGroupName)
        {
            this.GroupName = rGroupName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class SBGroupIneritedAttribute : SBGroupAttribute
    {
        public SBGroupIneritedAttribute(string rGroupName)
            : base(rGroupName)
        {
        }
    }
}