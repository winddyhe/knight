using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Model
{
	public static class DllHelper
	{
        public static Type[] GetMonoTypes()
		{
			List<Type> types = new List<Type>();
            foreach (var rAssembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                types.AddRange(rAssembly.GetTypes());
            }
            return types.ToArray();
		}
	}
}