using System;
using System.Collections.Generic;
using System.Reflection;

namespace Knight.Framework.Serializer
{
    public class SerializerAssists
    {
        public static List<MemberInfo> FindSerializeMembers(Type rType)
        {
            var rMemberInfos = new List<MemberInfo>();
            foreach (var rMemberInfo in rType.GetMembers())
            {
                if ((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    rMemberInfo.DeclaringType != rType)
                    continue;

                if (rMemberInfo.IsDefined(typeof(SBIgnoreAttribute), false))
                    continue;

                if (rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    Knight.Core.LogManager.LogErrorFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }

            foreach (var rMemberInfo in rType.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if ((rMemberInfo.MemberType != MemberTypes.Field &&
                    rMemberInfo.MemberType != MemberTypes.Property) ||
                    rMemberInfo.DeclaringType != rType)
                    continue;

                if (!rMemberInfo.IsDefined(typeof(SBEnableAttribute), false))
                    continue;

                if (rMemberInfo.MemberType == MemberTypes.Property &&
                    (!(rMemberInfo as PropertyInfo).CanRead || !(rMemberInfo as PropertyInfo).CanWrite))
                {
                    Knight.Core.LogManager.LogErrorFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
                    continue;
                }

                rMemberInfos.Add(rMemberInfo);
            }
            return rMemberInfos;
        }


        public static string GetClassMemberDummyText(MemberInfo rMemberInfo)
        {   
            if (rMemberInfo.MemberType == MemberTypes.Field)
            {
                string rConvertTo;
                var rFieldType = (rMemberInfo as FieldInfo).FieldType;
                if (rFieldType.IsEnum && Enum.GetUnderlyingType(rFieldType) == typeof(long))
                    rConvertTo = "(long)";
                else
                    rConvertTo = "(int)";
                return rFieldType.IsEnum ?
                    $"{rConvertTo}this.{rMemberInfo.Name}" :
                    $"this.{rMemberInfo.Name}";
            }
            else if (rMemberInfo.MemberType == MemberTypes.Property)
            {
                string rConvertTo;
                var rPropertyType = (rMemberInfo as PropertyInfo).PropertyType;
                if (rPropertyType.IsEnum && Enum.GetUnderlyingType(rPropertyType) == typeof(long))
                    rConvertTo = "(long)";
                else
                    rConvertTo = "(int)";
                return rPropertyType.IsEnum ?
                    $"{rConvertTo}this.{rMemberInfo.Name}" :
                    $"this.{rMemberInfo.Name}";
            }

            return string.Empty;
        }

        public static string GetTypeName(Type rType)
        {
            if (rType == typeof(char)) return "char";
            else if (rType == typeof(byte)) return "byte";
            else if (rType == typeof(sbyte)) return "sbyte";
            else if (rType == typeof(short)) return "short";
            else if (rType == typeof(ushort)) return "ushort";
            else if (rType == typeof(int)) return "int";
            else if (rType == typeof(uint)) return "uint";
            else if (rType == typeof(long)) return "long";
            else if (rType == typeof(ulong)) return "ulong";
            else if (rType == typeof(float)) return "float";
            else if (rType == typeof(double)) return "double";
            else if (rType == typeof(decimal)) return "decimal";
            else if (rType == typeof(string)) return "string";
            else if (rType == typeof(void)) return "void";
            else if (rType.IsArray)
            {
                return string.Format("{0}[]", GetTypeName(rType.GetElementType()));
            }
            else if (rType.GetInterface("System.Collections.IList") != null)
            {
                return string.Format("List<{0}>", GetTypeName(rType.GetGenericArguments()[0]));
            }
            else if (rType.FullName.StartsWith("Knight.Core.Dict`2"))
            {
                return string.Format("Dict<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.GetInterface("System.Collections.IDictionary") != null)
            {
                return string.Format("Dictionary<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.FullName.StartsWith("Knight.Core.CTGSEnumDictionary`2"))
            {
                return string.Format("Knight.Core.CTGSEnumDictionary<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.FullName.StartsWith("System.Action`1"))
            {
                var rArgType = rType.GetGenericArguments()[0];
                if (rArgType.FullName.StartsWith("System.Collections.Generic.List`1"))
                    return string.Format("System.Action<List<{0}>>", GetTypeName(rArgType.GetGenericArguments()[0]));
                else
                    return string.Format("System.Action<{0}>", GetTypeName(rArgType));
            }
            else if (rType.FullName.StartsWith("System.Action`2"))
            {
                return string.Format("System.Action<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.FullName.StartsWith("System.Collections.Generic.KeyValuePair`2"))
            {
                return string.Format("KeyValuePair<{0}, {1}>",
                    GetTypeName(rType.GetGenericArguments()[0]),
                    GetTypeName(rType.GetGenericArguments()[1]));
            }
            else if (rType.FullName.StartsWith("Knight.Core.KeyValue`2"))
            {
                return string.Format("KeyValue<{0}, {1}>",
                     GetTypeName(rType.GetGenericArguments()[0]),
                     GetTypeName(rType.GetGenericArguments()[1]));
            }
            else
            {
                return rType.FullName.Replace('+', '.');
            }
        }

        public static object GetDeserializeUnwrap(Type rType)
        {
            if (rType == typeof(char)) return "default(char)";
            else if (rType == typeof(byte)) return "default(byte)";
            else if (rType == typeof(sbyte)) return "default(sbyte)";
            else if (rType == typeof(short)) return "default(short)";
            else if (rType == typeof(ushort)) return "default(ushort0";
            else if (rType == typeof(int)) return "default(int)";
            else if (rType == typeof(uint)) return "default(uint)";
            else if (rType == typeof(long)) return "default(long)";
            else if (rType == typeof(ulong)) return "default(ulong)";
            else if (rType == typeof(float)) return "default(float)";
            else if (rType == typeof(double)) return "default(double)";
            else if (rType == typeof(decimal)) return "default(decimal)";
            else if (rType == typeof(string)) return "string.Empty";
            else if (rType.IsEnum)
            {
                return string.Format("int.MaxValue");
            }
            else
            {
                return string.Format("default({0})", GetTypeName(rType));
            }
        }

        public static bool IsBaseType(Type rType, bool bIncludeSB = true)
        {
            return
                (rType == typeof(char)) || (rType == typeof(byte)) || (rType == typeof(sbyte)) ||
                (rType == typeof(short) || (rType == typeof(ushort)) || (rType == typeof(int)) ||
                (rType == typeof(uint)) || (rType == typeof(long)) || (rType == typeof(ulong)) ||
                (rType == typeof(float)) || (rType == typeof(double)) || (rType == typeof(decimal)) ||
                (rType == typeof(string)) || (bIncludeSB && rType.IsDefined(typeof(SerializerBinaryAttribute), false)));
        }

        public static Type GetMemberType(MemberInfo rMemberInfo)
        {
            if (rMemberInfo.MemberType == MemberTypes.Field)
                return (rMemberInfo as FieldInfo).FieldType;
            else if (rMemberInfo.MemberType == MemberTypes.Property)
                return (rMemberInfo as PropertyInfo).PropertyType;

            return null;
        }

        public static string GetClassMemberTypeText(MemberInfo rMemberInfo)
        {
            var rMemberType = GetMemberType(rMemberInfo);
            if (null != rMemberType)
                return rMemberType.IsEnum ? string.Format("({0})", rMemberType.FullName) : string.Empty;

            return string.Empty;
        }


    }
}
