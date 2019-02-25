// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;
#if NET_4_6 || NET_STANDARD_2_0
using System.Threading.Tasks;
#endif

namespace UnityFx.Async.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="UnityWebRequest"/>.
	/// </summary>
	public static class UnityWebRequestExtensions
	{
		#region interface

		/// <summary>
		/// Creates an <see cref="IAsyncOperation"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static IAsyncOperation ToAsync(this UnityWebRequest request)
		{
			var result = new Helpers.WebRequestResult<object>(request);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an <see cref="IAsyncOperation{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static IAsyncOperation<T> ToAsync<T>(this UnityWebRequest request) where T : class
		{
			var result = new Helpers.WebRequestResult<T>(request);
			result.Start();
			return result;
		}

#if NET_4_6 || NET_STANDARD_2_0

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task ToTask(this UnityWebRequest request)
		{
			var result = new TaskCompletionSource<object>(request);
			AsyncUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request));
			return result.Task;
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the specified <see cref="UnityWebRequest"/>.
		/// </summary>
		/// <param name="request">The source web request.</param>
		public static Task<T> ToTask<T>(this UnityWebRequest request) where T : class
		{
			var result = new TaskCompletionSource<T>(request);
			AsyncUtility.AddCompletionCallback(request, () => OnTaskCompleted(result, request));
			return result.Task;
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.UnityWebRequestAwaiter GetAwaiter(this UnityWebRequest op)
		{
			return new CompilerServices.UnityWebRequestAwaiter(op);
		}

#endif

		#endregion

		#region implementation

#if NET_4_6 || NET_STANDARD_2_0

		private static void OnTaskCompleted<T>(TaskCompletionSource<T> tcs, UnityWebRequest request) where T : class
		{
			try
			{
#if UNITY_5
				if (request.isError)
#else
				if (request.isHttpError || request.isNetworkError)
#endif
				{
					tcs.TrySetException(new WebRequestException(request.error, request.responseCode));
				}
				else if (request.downloadHandler != null)
				{
					var result = AsyncWww.GetResult<T>(request);
					tcs.TrySetResult(result);
				}
				else
				{
					tcs.TrySetResult(null);
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(e);
			}
		}

#endif

		#endregion
	}
}
