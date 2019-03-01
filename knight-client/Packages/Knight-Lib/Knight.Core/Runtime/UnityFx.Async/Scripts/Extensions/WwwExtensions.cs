// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
#if NET_4_6 || NET_STANDARD_2_0
using System.Threading.Tasks;
#endif

namespace UnityFx.Async.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="WWW"/>.
	/// </summary>
	public static class WwwExtensions
	{
#if !UNITY_2018_3_OR_NEWER

		#region interface

		/// <summary>
		/// Creates an <see cref="IAsyncOperation"/> wrapper for the specified <see cref="WWW"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static IAsyncOperation ToAsync(this WWW request)
		{
			var result = new Helpers.WwwResult<object>(request);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an <see cref="IAsyncOperation{TResult}"/> wrapper for the specified <see cref="WWW"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static IAsyncOperation<T> ToAsync<T>(this WWW request) where T : class
		{
			var result = new Helpers.WwwResult<T>(request);
			result.Start();
			return result;
		}

#if NET_4_6 || NET_STANDARD_2_0

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="WWW"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task ToTask(this WWW request)
		{
			var result = new TaskCompletionSource<object>(request);
			AsyncUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request));
			return result.Task;
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="WWW"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task<T> ToTask<T>(this WWW request) where T : class
		{
			var result = new TaskCompletionSource<T>(request);
			AsyncUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request));
			return result.Task;
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.WwwAwaiter GetAwaiter(this WWW op)
		{
			return new CompilerServices.WwwAwaiter(op);
		}

#endif

		#endregion

		#region implementation

#if NET_4_6 || NET_STANDARD_2_0

		private static void OnTaskCompleted<T>(TaskCompletionSource<T> tcs, WWW www) where T : class
		{
			try
			{
				if (string.IsNullOrEmpty(www.error))
				{
					var result = AsyncWww.GetResult<T>(www);
					tcs.TrySetResult(result);
				}
				else
				{
					tcs.TrySetException(new WebRequestException(www.error));
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		}

#endif

	#endregion

#endif
	}
}
