using System.IO;
using System;
using Knight.Core;

namespace Knight.Framework.Serializer
{
    public interface ISerializerBinary
    {
        void Serialize(BinaryWriter rWriter);
        void Deserialize(BinaryReader rReader);
    }

    public interface ISBReadWriteFile
    {
        void Save(string rFilePath);
        void Read(byte[] rBytes);
    }

    public class TSerializerBinaryHelper<T> where T : ISerializerBinary
    {
        public static void Serialize(T rSBObject, BinaryWriter rWriter)
        {
            rSBObject.Serialize(rWriter);
        }

        public static void Deserialize(T rSBObject, BinaryReader rReader)
        {
            rSBObject.Deserialize(rReader);
        }
    }

    public class TSerializerBinaryReadWriteHelper
    {
        public static void Save<T>(T rSBObject, string rFilePath) where T : new()
        {
            ((ISBReadWriteFile)rSBObject)?.Save(rFilePath);
        }

        public static void Read<T>(T rSBObject, byte[] rBytes) where T : new()
        {
            ((ISBReadWriteFile)rSBObject)?.Read(rBytes);
        }
    }

    public class SerializerBinaryTypes : TypeSearchDefault<ISerializerBinary> { }

    [AttributeUsage(AttributeTargets.Class)]
    public class SerializerBinaryAttribute : Attribute { }

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

    public class SBFileReadWriteAttribute : Attribute
    {
        public SBFileReadWriteAttribute()
        {
        }
    }
}