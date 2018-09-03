// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif
#if NET_4_6 || NET_STANDARD_2_0
using System.Threading.Tasks;
#endif

namespace UnityFx.Async
{
	/// <summary>
	/// Extensions for Unity API.
	/// </summary>
	public static class UnityExtensions
	{
		#region AsyncOperation

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
		/// Provides an object that waits for the completion of an <see cref="AsyncOperation"/>. This type and its members are intended for compiler use only.
		/// </summary>
		public struct AsyncOperationAwaiter : INotifyCompletion
		{
			private readonly AsyncOperation _op;

			/// <summary>
			/// Initializes a new instance of the <see cref="AsyncOperationAwaiter"/> struct.
			/// </summary>
			public AsyncOperationAwaiter(AsyncOperation op)
			{
				_op = op;
			}

			/// <summary>
			/// Gets a value indicating whether the underlying operation is completed.
			/// </summary>
			/// <value>The operation completion flag.</value>
			public bool IsCompleted => _op.isDone;

			/// <summary>
			/// Returns the source result value.
			/// </summary>
			public void GetResult()
			{
			}

			/// <inheritdoc/>
			public void OnCompleted(Action continuation)
			{
				AsyncUtility.AddCompletionCallback(_op, continuation);
			}
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler rather than use directly in code.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation op)
		{
			return new AsyncOperationAwaiter(op);
		}

#endif

		#endregion

		#region UnityWebRequest

#if UNITY_5_4_OR_NEWER

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
		/// Provides an object that waits for the completion of an <see cref="UnityWebRequest"/>. This type and its members are intended for compiler use only.
		/// </summary>
		public struct UnityWebRequestAwaiter : INotifyCompletion
		{
			private readonly UnityWebRequest _op;

			/// <summary>
			/// Initializes a new instance of the <see cref="UnityWebRequestAwaiter"/> struct.
			/// </summary>
			public UnityWebRequestAwaiter(UnityWebRequest op)
			{
				_op = op;
			}

			/// <summary>
			/// Gets a value indicating whether the underlying operation is completed.
			/// </summary>
			/// <value>The operation completion flag.</value>
			public bool IsCompleted => _op.isDone;

			/// <summary>
			/// Returns the source result value.
			/// </summary>
			public void GetResult()
			{
			}

			/// <inheritdoc/>
			public void OnCompleted(Action continuation)
			{
				AsyncUtility.AddCompletionCallback(_op, continuation);
			}
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler rather than use directly in code.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequest op)
		{
			return new UnityWebRequestAwaiter(op);
		}

#endif

#endif

		#endregion

		#region WWW

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
		/// Provides an object that waits for the completion of an <see cref="WWW"/>. This type and its members are intended for compiler use only.
		/// </summary>
		public struct WwwAwaiter : INotifyCompletion
		{
			private readonly WWW _op;

			/// <summary>
			/// Initializes a new instance of the <see cref="WwwAwaiter"/> struct.
			/// </summary>
			public WwwAwaiter(WWW op)
			{
				_op = op;
			}

			/// <summary>
			/// Gets a value indicating whether the underlying operation is completed.
			/// </summary>
			/// <value>The operation completion flag.</value>
			public bool IsCompleted => _op.isDone;

			/// <summary>
			/// Returns the source result value.
			/// </summary>
			public void GetResult()
			{
			}

			/// <inheritdoc/>
			public void OnCompleted(Action continuation)
			{
				AsyncUtility.AddCompletionCallback(_op, continuation);
			}
		}

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler rather than use directly in code.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static WwwAwaiter GetAwaiter(this WWW op)
		{
			return new WwwAwaiter(op);
		}

#endif

		#endregion

		#region Animation

		/// <summary>
		/// Plays an animation without any blending.
		/// </summary>
		/// <param name="anim">Target animation instance.</param>
		/// <param name="mode">The mode which lets you choose how this animation will affect others already playing.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animation anim, PlayMode mode)
		{
			if (anim.clip)
			{
				var result = new Helpers.PlayAnimationResult(anim, anim.clip.name, mode, AsyncUtility.GetUpdateSource());
				result.Start();
				return result;
			}

			return AsyncResult.CanceledOperation;
		}

		/// <summary>
		/// Plays an animation without any blending.
		/// </summary>
		/// <param name="anim">Target animation instance.</param>
		/// <param name="animation">Name of the animation to play.</param>
		/// <param name="mode">The mode which lets you choose how this animation will affect others already playing.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animation anim, string animation, PlayMode mode)
		{
			var result = new Helpers.PlayAnimationResult(anim, animation, mode, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		#endregion

		#region Animator

		/// <summary>
		/// Plays an animator state.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <param name="stateName">The state name.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animator anim, string stateName, int layer)
		{
			var result = new Helpers.PlayAnimatorResult(anim, stateName, layer, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		/// <summary>
		/// Plays an animator state.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <param name="stateName">The state name.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animator anim, string stateName)
		{
			var result = new Helpers.PlayAnimatorResult(anim, stateName, -1, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		/// <summary>
		/// Waits until current animation completes.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <param name="layer">The layer index.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation WaitAsync(this Animator anim, int layer)
		{
			var result = new Helpers.WaitAnimatorResult(anim, layer, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		/// <summary>
		/// Waits until current animation completes.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation WaitAsync(this Animator anim)
		{
			var result = new Helpers.WaitAnimatorResult(anim, 0, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		#endregion

		#region implementation

#if NET_4_6 || NET_STANDARD_2_0

#if UNITY_5_4_OR_NEWER

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
					tcs.TrySetException(new Helpers.WebRequestException(request.error, request.responseCode));
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
					tcs.TrySetException(new Helpers.WebRequestException(www.error));
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
