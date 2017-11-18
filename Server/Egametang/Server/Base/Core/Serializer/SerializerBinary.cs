using System.IO;
using System;
using Core;

namespace Core.Serializer
{
    /// <summary>
    /// SerializerBinary
    ///    Public Field/Public Property(get;set)
    /// </summary>
    [TSIgnore]
    public class SerializerBinary
    {
        public virtual void Serialize(BinaryWriter rWriter) { }
        public virtual void Deserialize(BinaryReader rReader) { }
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
        {}
    }

    /// <summary>
    /// ValueTypeSerialize
    /// </summary>
    public static class ValueTypeSerialize
    {
        public static void Serialize(this BinaryWriter rWriter, char value)     { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, byte value)     { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, sbyte value)    { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, bool value)     { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, short value)    { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, ushort value)   { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, int value)      { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, uint value)     { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, long value)     { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, ulong value)    { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, float value)    { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, double value)   { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, decimal value)  { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, string value)
        {
            bool bValid = !string.IsNullOrEmpty(value);
            rWriter.Write(bValid);
            if (bValid)
                rWriter.Write(value);
        }
    }

    /// <summary>
    /// ValueTypeDeserialize
    /// </summary>
    public static class ValueTypeDeserialize
    {
        public static char      Deserialize(this BinaryReader rReader, char value)    { return rReader.ReadChar();    }
        public static byte      Deserialize(this BinaryReader rReader, byte value)    { return rReader.ReadByte();    }
        public static sbyte     Deserialize(this BinaryReader rReader, sbyte value)   { return rReader.ReadSByte();   }
        public static bool      Deserialize(this BinaryReader rReader, bool value)    { return rReader.ReadBoolean(); }
        public static short     Deserialize(this BinaryReader rReader, short value)   { return rReader.ReadInt16();   }
        public static ushort    Deserialize(this BinaryReader rReader, ushort value)  { return rReader.ReadUInt16();  }
        public static int       Deserialize(this BinaryReader rReader, int value)     { return rReader.ReadInt32();   }
        public static uint      Deserialize(this BinaryReader rReader, uint value)    { return rReader.ReadUInt32();  }
        public static long      Deserialize(this BinaryReader rReader, long value)    { return rReader.ReadInt64();   }
        public static ulong     Deserialize(this BinaryReader rReader, ulong value)   { return rReader.ReadUInt64();  }
        public static float     Deserialize(this BinaryReader rReader, float value)   { return rReader.ReadSingle();  }
        public static double    Deserialize(this BinaryReader rReader, double value)  { return rReader.ReadDouble();  }
        public static decimal   Deserialize(this BinaryReader rReader, decimal value) { return rReader.ReadDecimal(); }
        public static string    Deserialize(this BinaryReader rReader, string value)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return string.Empty;
            return rReader.ReadString();
        }
    }

    /// <summary>
    /// SerializerBinarySerialize
    /// </summary>
    public static class SerializerBinarySerialize
    {
        public static void Serialize<T>(this BinaryWriter rWriter, T rValue)
            where T : SerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
                rValue.Serialize(rWriter);
        }
        public static void SerializeDynamic<T>(this BinaryWriter rWriter, T rValue)
            where T : SerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.GetType().FullName);
                rValue.Serialize(rWriter);
            }
        }
    }

    /// <summary>
    /// SerializerBinaryDeserialize
    /// </summary>
    public static class SerializerBinaryDeserialize
    {
        public static T Deserialize<T>(this BinaryReader rReader, T value)
            where T : SerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return null;

            var rInstance = ReflectExpand.Construct<T>();
            rInstance.Deserialize(rReader);
            return rInstance;
        }
        public static T DeserializeDynamic<T>(this BinaryReader rReader, T rValue)
            where T : SerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return null;

            var rFullName = rReader.Deserialize(string.Empty);
            var rInstance = ReflectExpand.TConstruct<T>(Type.GetType(rFullName));
            rInstance.Deserialize(rReader);
            return rInstance;
        }
    }
}