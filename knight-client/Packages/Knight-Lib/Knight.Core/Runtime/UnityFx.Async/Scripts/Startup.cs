// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using UnityEngine;

namespace UnityFx.Async
{
	/// <summary>
	/// Manages initialization of library services. For internal use only.
	/// </summary>
	internal static class Startup
	{
		private static GameObject _go;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize()
		{
			var context = SynchronizationContext.Current;

			if (context == null)
			{
				// Create custom SynchronizationContext for the main thread.
				context = new Helpers.MainThreadSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(context);
			}

			// Set main thread context as default for all continuations. This saves allocations in many cases.
			AsyncResult.DefaultSynchronizationContext = context;

			// Create helper GameObject
			_go = new GameObject("UnityFx.Async")
			{
				hideFlags = HideFlags.HideAndDontSave
			};

			GameObject.DontDestroyOnLoad(_go);

			// Initialize library components.
			AsyncUtility.Initialize(_go, context);
			AsyncWww.Initialize(_go, context);
		}
	}
}
