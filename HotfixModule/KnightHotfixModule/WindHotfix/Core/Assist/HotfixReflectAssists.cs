using System;
using System.Collections.Generic;

namespace WindHotfix.Core
{
    public class HotfixReflectAssists
    {
        public static object Construct(Type rType, params object[] param)
        {
            var rParamType = new Type[param.Length];
            for (int nIndex = 0; nIndex < param.Length; ++nIndex)
                rParamType[nIndex] = param[nIndex].GetType();

            var rConstructor = rType.GetConstructor(rParamType);
            return rConstructor.Invoke(param);
        }

        public static T Construct<T>(params object[] param)
        {
            return (T)Construct(typeof(T), param);
        }
        public static T TConstruct<T>(Type rType, params object[] param)
        {
            return (T)Construct(rType, param);
        }
    }
}
