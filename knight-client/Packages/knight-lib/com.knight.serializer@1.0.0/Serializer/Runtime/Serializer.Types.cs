using Knight.Core;
using System;
using System.IO;
using UnityEngine;

namespace Knight.Framework.Serializer
{
    /// <summary>
    /// ValueTypeSerialize
    /// </summary>
    public static class ValueTypeSerialize
    {
        public static void Serialize(this BinaryWriter rWriter, char value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, byte value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, sbyte value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, bool value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, short value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, ushort value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, int value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, uint value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, long value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, ulong value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, float value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, double value) { rWriter.Write(value); }
        public static void Serialize(this BinaryWriter rWriter, decimal value) { rWriter.Write(value); }
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
        public static char Deserialize(this BinaryReader rReader, char value) { return rReader.ReadChar(); }
        public static byte Deserialize(this BinaryReader rReader, byte value) { return rReader.ReadByte(); }
        public static sbyte Deserialize(this BinaryReader rReader, sbyte value) { return rReader.ReadSByte(); }
        public static bool Deserialize(this BinaryReader rReader, bool value) { return rReader.ReadBoolean(); }
        public static short Deserialize(this BinaryReader rReader, short value) { return rReader.ReadInt16(); }
        public static ushort Deserialize(this BinaryReader rReader, ushort value) { return rReader.ReadUInt16(); }
        public static int Deserialize(this BinaryReader rReader, int value) { return rReader.ReadInt32(); }
        public static uint Deserialize(this BinaryReader rReader, uint value) { return rReader.ReadUInt32(); }
        public static long Deserialize(this BinaryReader rReader, long value) { return rReader.ReadInt64(); }
        public static ulong Deserialize(this BinaryReader rReader, ulong value) { return rReader.ReadUInt64(); }
        public static float Deserialize(this BinaryReader rReader, float value) { return rReader.ReadSingle(); }
        public static double Deserialize(this BinaryReader rReader, double value) { return rReader.ReadDouble(); }
        public static decimal Deserialize(this BinaryReader rReader, decimal value) { return rReader.ReadDecimal(); }
        public static string Deserialize(this BinaryReader rReader, string value)
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
            where T : ISerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
                rValue.Serialize(rWriter);
        }
        public static void SerializeDynamic<T>(this BinaryWriter rWriter, T rValue)
            where T : ISerializerBinary
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.GetType().FullName);
                rValue.Serialize(rWriter);
            }
        }
        public static void Serialize(this BinaryWriter rWriter, Vector2 rValue)
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.x);
                rWriter.Serialize(rValue.y);
            }
        }
        public static void Serialize(this BinaryWriter rWriter, Vector3 rValue)
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.x);
                rWriter.Serialize(rValue.y);
                rWriter.Serialize(rValue.z);
            }
        }
        public static void Serialize(this BinaryWriter rWriter, Vector4 rValue)
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.x);
                rWriter.Serialize(rValue.y);
                rWriter.Serialize(rValue.z);
                rWriter.Serialize(rValue.w);
            }
        }
        public static void Serialize(this BinaryWriter rWriter, Color rValue)
        {
            bool bValid = null != rValue;
            rWriter.Serialize(bValid);
            if (bValid)
            {
                rWriter.Serialize(rValue.r);
                rWriter.Serialize(rValue.g);
                rWriter.Serialize(rValue.b);
                rWriter.Serialize(rValue.a);
            }
        }
    }

    /// <summary>
    /// SerializerBinaryDeserialize
    /// </summary>
    public static class SerializerBinaryDeserialize
    {
        public static T Deserialize<T>(this BinaryReader rReader, T value)
            where T : ISerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return default(T);

            var rInstance = ReflectTool.Construct<T>();
            rInstance.Deserialize(rReader);
            return rInstance;
        }
        public static T DeserializeDynamic<T>(this BinaryReader rReader, T rValue)
            where T : ISerializerBinary
        {
            bool bValid = rReader.Deserialize(false);
            if (!bValid)
                return default(T);

            var rFullName = rReader.Deserialize(string.Empty);
            var rInstance = ReflectTool.TConstruct<T>(Type.GetType(rFullName));
            rInstance.Deserialize(rReader);
            return rInstance;
        }
        public static Vector2 Deserialize(this BinaryReader rReader, Vector2 rValue)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return Vector2.zero;

            Vector2 rVec2 = Vector2.zero;
            rVec2.x = rReader.ReadSingle();
            rVec2.y = rReader.ReadSingle();
            return rVec2;
        }
        public static Vector3 Deserialize(this BinaryReader rReader, Vector3 rValue)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return Vector3.zero;

            Vector3 rVec3 = Vector3.zero;
            rVec3.x = rReader.ReadSingle();
            rVec3.y = rReader.ReadSingle();
            rVec3.z = rReader.ReadSingle();
            return rVec3;
        }
        public static Vector4 Deserialize(this BinaryReader rReader, Vector4 rValue)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return Vector4.zero;

            Vector4 rVec3 = Vector4.zero;
            rVec3.x = rReader.ReadSingle();
            rVec3.y = rReader.ReadSingle();
            rVec3.z = rReader.ReadSingle();
            rVec3.w = rReader.ReadSingle();
            return rVec3;
        }
        public static Color Deserialize(this BinaryReader rReader, Color rValue)
        {
            bool bValid = rReader.ReadBoolean();
            if (!bValid)
                return Color.black;

            Color rVec3 = Color.black;
            rVec3.r = rReader.ReadSingle();
            rVec3.g = rReader.ReadSingle();
            rVec3.b = rReader.ReadSingle();
            rVec3.a = rReader.ReadSingle();
            return rVec3;
        }
    }
}
