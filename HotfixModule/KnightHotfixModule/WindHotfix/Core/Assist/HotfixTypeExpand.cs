using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WindHotfix.Core
{
    public static class HotfixTypeExpand
    {
        public static Type HotfixSearchBaseTo(this Type rType, Type rBaseType)
        {
            var rSearchType = rType;
            while (rSearchType.BaseType != rBaseType && null != rSearchType.BaseType)
            {
                rSearchType = rSearchType.BaseType;
            }
            return rSearchType.BaseType == rBaseType ? rSearchType : null;
        }

        public static Type HotfixSearchBaseTo<T>(this Type rType)
        {
            return HotfixSearchBaseTo(rType, typeof(T));
        }

        public static bool HotfixIsApplyAttr<T>(this ICustomAttributeProvider rProvider, bool bInherit)
        {
            return rProvider.GetCustomAttributes(typeof(T), bInherit).Length > 0;
        }
        public static bool HotfixIsApplyAttr(this ICustomAttributeProvider rProvider, Type rType, bool bInherit)
        {
            return rProvider.GetCustomAttributes(rType, bInherit).Length > 0;
        }
    }
}
