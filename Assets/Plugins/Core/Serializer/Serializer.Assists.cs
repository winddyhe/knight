//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core.Serializer
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
                    Debug.LogFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
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
                    Debug.LogFormat("{0}.{1} Skip Serialize!", rType.FullName, rMemberInfo.Name);
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
                return (rMemberInfo as FieldInfo).FieldType.IsEnum ?
                    string.Format("(int)this.{0}", rMemberInfo.Name) :
                    string.Format("this.{0}", rMemberInfo.Name);
            }
            else if (rMemberInfo.MemberType == MemberTypes.Property)
            {
                return (rMemberInfo as PropertyInfo).PropertyType.IsEnum ?
                    string.Format("(int)this.{0}", rMemberInfo.Name) :
                    string.Format("this.{0}", rMemberInfo.Name);
            }

            return string.Empty;
        }

        public static bool IsBaseType(Type rType, bool bIncludeSB = true)
        {
            return
                (rType == typeof(char))   || (rType == typeof(byte))   || (rType == typeof(sbyte))   ||
                (rType == typeof(short)   || (rType == typeof(ushort)) || (rType == typeof(int))     ||
                (rType == typeof(uint))   || (rType == typeof(long))   || (rType == typeof(ulong))   ||
                (rType == typeof(float))  || (rType == typeof(double)) || (rType == typeof(decimal)) ||
                (rType == typeof(string)) || (bIncludeSB && typeof(SerializerBinary).IsAssignableFrom(rType)));
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
