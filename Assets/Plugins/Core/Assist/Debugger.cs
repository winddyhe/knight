//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using UnityEngine;
using System.Collections;
using System;

namespace Core
{
    public static class Debugger
    {
        public static bool Assert(bool bCondition, string format, params object[] args)
        {
            if (!bCondition)
                Debug.LogError(string.Format(null, format, args));

            return bCondition;
        }
        public static void AssertE(bool bCondition, string format, params object[] args)
        {
            if (!bCondition)
                throw new UnityException(string.Format(null, format, args));
        }
        public static void ValidValueE(string value, string format, params object[] args)
        {
            if(string.IsNullOrEmpty(value))
                throw new UnityException(string.Format(null, format, args));
        }
        public static void ValidValueE<T>(T value, string format, params object[] args)
            where T : class
        {
            if(null == value)
                throw new UnityException(string.Format(null, format, args));
        }
    }
	public class DebuggerMethodClass {}
	public class DebuggerMethodClassTypes : TypeSearchDefault<DebuggerMethodClass> {}

	[AttributeUsage(AttributeTargets.Method)]
	public class DebuggerMethodAttribute : System.Attribute
	{
	}
	
	[AttributeUsage(AttributeTargets.Parameter)]
	public class DebuggerMethodParamAttribute : System.Attribute
	{
		public int    IMin = int.MinValue;
		public int    IMax = int.MaxValue;

		public float  FMin = float.MinValue;
		public float  FMax = float.MaxValue;

		public string DefaultValue = string.Empty;

		public bool   UserRange = false;

		public DebuggerMethodParamAttribute()
		{
		}
		public DebuggerMethodParamAttribute(string rDefault)
		{
			DefaultValue = rDefault;
		}
		public DebuggerMethodParamAttribute(int nDefault, int nMin = int.MinValue, int nMax = int.MaxValue)
		{
			DefaultValue = nDefault.ToString();

			IMin = nMin; IMax = nMax;

			UserRange = true;
		}
		public DebuggerMethodParamAttribute(float nDefault, float nMin = float.MinValue, float nMax = float.MaxValue)
		{
			DefaultValue = nDefault.ToString();

			FMin = nMin; FMax = nMax;

			UserRange = true;
		}
		public DebuggerMethodParamAttribute(Type rDefault)
		{
			DefaultValue = rDefault.FullName;
		}
	}

	public class DebuggerMethodRuntime : DebuggerMethodClass
	{
		[DebuggerMethod()]
		public static void PrintToConsole([DebuggerMethodParam("Print this text!")] string PrintText,
		                                  [DebuggerMethodParam(5, 0, 100)]int NumberValue,
		                                  [DebuggerMethodParam(0.5f, -1.0f, 1.0f)]float RealValue)
		{
			UnityEngine.Debug.Log(PrintText);
		}

		[DebuggerMethod()]
		public static void Command(string rCommands)
		{
		}
	}
}