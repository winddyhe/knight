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
	/// Extension methods for <see cref="AsyncOperation"/>.
	/// </summary>
	public static class AsyncOperationExtensions
	{
		/// <summary>
		/// Creates an <see cref="IAsyncOperation"/> wrapper for the Unity <see cref="AsyncOperation"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static IAsyncOperation ToAsync(this AsyncOperation op)
		{
			if (op.isDone)
			{
				return AsyncResult.CompletedOperation;
			}
			else
			{
				var result = new Helpers.AsyncOperationResult(op);
				result.Start();
				return result;
			}
		}

		/// <summary>
		/// Creates an <see cref="IAsyncOperation{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static IAsyncOperation<T> ToAsync<T>(this ResourceRequest op) where T : UnityEngine.Object
		{
			var result = new Helpers.ResourceRequestResult<T>(op);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an <see cref="IAsyncOperation{TResult}"/> wrapper for the Unity <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static IAsyncOperation<T> ToAsync<T>(this AssetBundleRequest op) where T : UnityEngine.Object
		{
			var result = new Helpers.AssetBundleRequestResult<T>(op);
			result.Start();
			return result;
		}

#if NET_4_6 || NET_STANDARD_2_0

		/// <summary>
		/// Creates an <see cref="Task"/> wrapper for the Unity <see cref="AsyncOperation"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static Task ToTask(this AsyncOperation op)
		{
			if (op.isDone)
			{
				return Task.CompletedTask;
			}
			else
			{
				var result = new TaskCompletionSource<object>(op);
				AsyncUtility.AddCompletionCallback(op, () => result.TrySetResult(null));
				return result.Task;
			}
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="ResourceRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static Task<T> ToTask<T>(this ResourceRequest op) where T : UnityEngine.Object
		{
			var result = new TaskCompletionSource<T>(op);
			AsyncUtility.AddCompletionCallback(op, () => result.TrySetResult(op.asset as T));
			return result.Task;
		}

		/// <summary>
		/// Creates an <see cref="Task{TResult}"/> wrapper for the Unity <see cref="AssetBundleRequest"/>.
		/// </summary>
		/// <param name="op">The source operation.</param>
		public static Task<T> ToTask<T>(this AssetBundleRequest op) where T : UnityEngine.Object
		{
			var result = new TaskCompletionSource<T>(op);
			AsyncUtility.AddCompletionCallback(op, () => result.TrySetResult(op.asset as T));
			return result.Task;
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
		{
			return new CompilerServices.AsyncOperationAwaiter(op);
		}

#endif
	}
}
