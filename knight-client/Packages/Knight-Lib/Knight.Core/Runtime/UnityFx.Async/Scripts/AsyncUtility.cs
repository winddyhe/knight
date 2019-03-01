// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
#if NET_4_6 || NET_STANDARD_2_0
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
#endif
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityFx.Async.Extensions;

namespace UnityFx.Async
{
	/// <summary>
	/// Utility classes.
	/// </summary>
	public static class AsyncUtility
	{
		#region data

		private static SynchronizationContext _mainThreadContext;
		private static AsyncUtilityBehaviour _rootBehaviour;

		#endregion

		#region interface

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for Update.
		/// </summary>
		/// <seealso cref="GetUpdateSource(FrameTiming)"/>
		public static IAsyncUpdateSource GetUpdateSource()
		{
			return _rootBehaviour.UpdateSource;
		}

		/// <summary>
		/// Returns an instance of an <see cref="IAsyncUpdateSource"/> for the specified <paramref name="frameTiming"/>.
		/// </summary>
		/// <seealso cref="GetUpdateSource()"/>
		public static IAsyncUpdateSource GetUpdateSource(FrameTiming frameTiming)
		{
			switch (frameTiming)
			{
				case FrameTiming.FixedUpdate:
					return _rootBehaviour.FixedUpdateSource;

				case FrameTiming.LateUpdate:
					return _rootBehaviour.LateUpdateSource;

				case FrameTiming.EndOfFrame:
					return _rootBehaviour.EofUpdateSource;
			}

			return _rootBehaviour.UpdateSource;
		}

		/// <summary>
		/// Register a completion callback that is triggered on a specific time during next frame.
		/// </summary>
		/// <param name="callback">A delegate to be called on the next frame.</param>
		/// <param name="timing">Time to call the <paramref name="callback"/>.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="callback"/> is <see langword="null"/>.</exception>
		public static void AddFrameCallback(Action callback, FrameTiming timing)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			_rootBehaviour.AddFrameCallback(callback, timing);
		}

		/// <summary>
		/// Dispatches a synchronous message to the main thread.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <see langword="null"/>.</exception>
		/// <seealso cref="SendToMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="PostToMainThread(Action)"/>
		/// <seealso cref="InvokeOnMainThread(Action)"/>
		public static void SendToMainThread(Action action)
		{
			_mainThreadContext.Send(action);
		}

		/// <summary>
		/// Dispatches a synchronous message to the main thread.
		/// </summary>
		/// <param name="d">The delegate to invoke.</param>
		/// <param name="state">The object passed to the delegate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="d"/> is <see langword="null"/>.</exception>
		/// <seealso cref="SendToMainThread(Action)"/>
		/// <seealso cref="PostToMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="InvokeOnMainThread(SendOrPostCallback, object)"/>
		public static void SendToMainThread(SendOrPostCallback d, object state)
		{
			_mainThreadContext.Send(d, state);
		}

		/// <summary>
		/// Dispatches an asynchronous message to the main thread.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <see langword="null"/>.</exception>
		/// <seealso cref="PostToMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="SendToMainThread(Action)"/>
		/// <seealso cref="InvokeOnMainThread(Action)"/>
		public static void PostToMainThread(Action action)
		{
			_mainThreadContext.Post(action);
		}

		/// <summary>
		/// Dispatches an asynchronous message to the main thread.
		/// </summary>
		/// <param name="d">The delegate to invoke.</param>
		/// <param name="state">The object passed to the delegate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="d"/> is <see langword="null"/>.</exception>
		/// <seealso cref="PostToMainThread(Action)"/>
		/// <seealso cref="SendToMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="InvokeOnMainThread(SendOrPostCallback, object)"/>
		public static void PostToMainThread(SendOrPostCallback d, object state)
		{
			_mainThreadContext.Post(d, state);
		}

		/// <summary>
		/// Dispatches the specified delegate on the main thread.
		/// </summary>
		/// <param name="action">The delegate to invoke.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="action"/> is <see langword="null"/>.</exception>
		/// <seealso cref="InvokeOnMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="SendToMainThread(Action)"/>
		/// <seealso cref="PostToMainThread(Action)"/>
		public static void InvokeOnMainThread(Action action)
		{
			_mainThreadContext.Invoke(action);
		}

		/// <summary>
		/// Dispatches the specified delegate on the main thread.
		/// </summary>
		/// <param name="d">The delegate to invoke.</param>
		/// <param name="state">The object passed to the delegate.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="d"/> is <see langword="null"/>.</exception>
		/// <seealso cref="InvokeOnMainThread(Action)"/>
		/// <seealso cref="SendToMainThread(SendOrPostCallback, object)"/>
		/// <seealso cref="PostToMainThread(SendOrPostCallback, object)"/>
		public static void InvokeOnMainThread(SendOrPostCallback d, object state)
		{
			_mainThreadContext.Invoke(d, state);
		}

		/// <summary>
		/// Checks whether current thread is the main Unity thread.
		/// </summary>
		/// <returns>Returns <see langword="true"/> if current thread is Unity main thread; <see langword="false"/> otherwise.</returns>
		public static bool IsMainThread()
		{
			return _mainThreadContext == SynchronizationContext.Current;
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="millisecondsDelay">The number of milliseconds to wait before completing the returned operation, or -1 to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="millisecondsDelay"/> is less than -1.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		/// <seealso cref="Delay(float)"/>
		/// <seealso cref="Delay(TimeSpan)"/>
		public static IAsyncOperation Delay(int millisecondsDelay)
		{
			return AsyncResult.Delay(millisecondsDelay, _rootBehaviour.UpdateSource);
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="secondsDelay">The number of seconds to wait before completing the returned operation, or -1 to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="secondsDelay"/> is less than -1.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		/// <seealso cref="Delay(int)"/>
		/// <seealso cref="Delay(TimeSpan)"/>
		public static IAsyncOperation Delay(float secondsDelay)
		{
			return AsyncResult.Delay(secondsDelay, _rootBehaviour.UpdateSource);
		}

		/// <summary>
		/// Creates an operation that completes after a time delay.
		/// </summary>
		/// <param name="delay">The time span to wait before completing the returned operation, or <c>TimeSpan.FromMilliseconds(-1)</c> to wait indefinitely.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="delay"/> represents a negative time interval other than <c>TimeSpan.FromMillseconds(-1)</c>.</exception>
		/// <returns>An operation that represents the time delay.</returns>
		/// <seealso cref="Delay(int)"/>
		/// <seealso cref="Delay(float)"/>
		public static IAsyncOperation Delay(TimeSpan delay)
		{
			return AsyncResult.Delay(delay, _rootBehaviour.UpdateSource);
		}

		/// <summary>
		/// Starts a coroutine and wraps it with <see cref="IAsyncOperation"/>.
		/// </summary>
		/// <param name="coroutineFunc">The coroutine delegate.</param>
		/// <param name="userState">User-defined state.</param>
		/// <returns>Returns the coroutine handle.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="coroutineFunc"/> is <see langword="null"/>.</exception>
		/// <seealso cref="FromCoroutine(Func{IAsyncCompletionSource, IEnumerator}, object)"/>
		public static IAsyncOperation FromCoroutine(Func<IAsyncCompletionSource, IEnumerator> coroutineFunc, object userState = null)
		{
			if (coroutineFunc == null)
			{
				throw new ArgumentNullException("coroutineFunc");
			}

			var result = new Helpers.CoroutineResult(_rootBehaviour, coroutineFunc, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Starts a coroutine and wraps it with <see cref="IAsyncOperation{TResult}"/>.
		/// </summary>
		/// <param name="coroutineFunc">The coroutine delegate.</param>
		/// <param name="userState">User-defined state.</param>
		/// <returns>Returns the coroutine handle.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="coroutineFunc"/> is <see langword="null"/>.</exception>
		/// <seealso cref="FromCoroutine{TResult}(Func{IAsyncCompletionSource{TResult}, IEnumerator}, object)"/>
		public static IAsyncOperation<TResult> FromCoroutine<TResult>(Func<IAsyncCompletionSource<TResult>, IEnumerator> coroutineFunc, object userState = null)
		{
			if (coroutineFunc == null)
			{
				throw new ArgumentNullException("coroutineFunc");
			}

			var result = new Helpers.CoroutineResult<TResult>(_rootBehaviour, coroutineFunc, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Starts a coroutine.
		/// </summary>
		/// <param name="enumerator">The coroutine to run.</param>
		/// <returns>Returns the coroutine handle.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="enumerator"/> is <see langword="null"/>.</exception>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static Coroutine StartCoroutine(IEnumerator enumerator)
		{
			if (enumerator == null)
			{
				throw new ArgumentNullException("enumerator");
			}

			return _rootBehaviour.StartCoroutine(enumerator);
		}

		/// <summary>
		/// Stops the specified coroutine.
		/// </summary>
		/// <param name="coroutine">The coroutine to stop.</param>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StopCoroutine(Coroutine coroutine)
		{
			if (coroutine != null && _rootBehaviour)
			{
				_rootBehaviour.StopCoroutine(coroutine);
			}
		}

		/// <summary>
		/// Stops the specified coroutine.
		/// </summary>
		/// <param name="enumerator">The coroutine to stop.</param>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopAllCoroutines"/>
		public static void StopCoroutine(IEnumerator enumerator)
		{
			if (enumerator != null && _rootBehaviour)
			{
				_rootBehaviour.StopCoroutine(enumerator);
			}
		}

		/// <summary>
		/// Stops all coroutines.
		/// </summary>
		/// <seealso cref="StartCoroutine(IEnumerator)"/>
		/// <seealso cref="StopCoroutine(Coroutine)"/>
		/// <seealso cref="StopCoroutine(IEnumerator)"/>
		public static void StopAllCoroutines()
		{
			if (_rootBehaviour)
			{
				_rootBehaviour.StopAllCoroutines();
			}
		}

		/// <summary>
		/// Register a completion callback for the specified <see cref="AsyncOperation"/> instance. If operation is completed
		/// the <paramref name="completionCallback"/> is executed synchronously.
		/// </summary>
		/// <param name="op">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="op"/> has completed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="op"/> or <paramref name="completionCallback"/> is <see langword="null"/>.</exception>
		public static void AddCompletionCallback(AsyncOperation op, Action completionCallback)
		{
			if (op == null)
			{
				throw new ArgumentNullException("op");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			if (op.isDone)
			{
				completionCallback();
			}
			else
			{
#if UNITY_2017_2_OR_NEWER

				// Starting with Unity 2017.2 there is AsyncOperation.completed event.
				op.completed += o => completionCallback();

#else

				_rootBehaviour.AddCompletionCallback(op, completionCallback);

#endif
			}
		}

		/// <summary>
		/// Register a completion callback for the specified <see cref="UnityWebRequest"/> instance.
		/// </summary>
		/// <param name="request">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="request"/> has completed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="completionCallback"/> is <see langword="null"/>.</exception>
		/// <seealso cref="AddCompletionCallback(AsyncOperation, Action)"/>
		public static void AddCompletionCallback(UnityWebRequest request, Action completionCallback)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			_rootBehaviour.AddCompletionCallback(request, completionCallback);
		}

#if !UNITY_2018_3_OR_NEWER

		/// <summary>
		/// Register a completion callback for the specified <see cref="WWW"/> instance.
		/// </summary>
		/// <param name="request">The request to register completion callback for.</param>
		/// <param name="completionCallback">A delegate to be called when the <paramref name="request"/> has completed.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> or <paramref name="completionCallback"/> is <see langword="null"/>.</exception>
		/// <seealso cref="AddCompletionCallback(AsyncOperation, Action)"/>
		public static void AddCompletionCallback(WWW request, Action completionCallback)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (completionCallback == null)
			{
				throw new ArgumentNullException("completionCallback");
			}

			_rootBehaviour.AddCompletionCallback(request, completionCallback);
		}

#endif

#if NET_4_6 || NET_STANDARD_2_0

		/// <summary>
		/// Provides an object that awaits for the specified <see cref="FrameTiming"/>. This type and its members are intended for compiler use only.
		/// </summary>
		/// <seealso cref="FrameAwaitable"/>
		public struct FrameAwaiter : INotifyCompletion
		{
			private readonly FrameTiming _frameTiming;

			/// <summary>
			/// Initializes a new instance of the <see cref="FrameAwaiter"/> struct.
			/// </summary>
			public FrameAwaiter(FrameTiming frameTiming)
			{
				_frameTiming = frameTiming;
			}

			/// <summary>
			/// Gets a value indicating whether the underlying operation is completed.
			/// </summary>
			/// <value>The operation completion flag.</value>
			public bool IsCompleted
			{
				get
				{
					return false;
				}
			}

			/// <summary>
			/// Returns the source result value.
			/// </summary>
			public void GetResult()
			{
			}

			/// <inheritdoc/>
			public void OnCompleted(Action continuation)
			{
				_rootBehaviour.AddFrameCallback(continuation, _frameTiming);
			}
		}

		/// <summary>
		/// Provides an awaitable object that allows awaits for the specified <see cref="FrameTiming"/>. This type is intended for compiler use only.
		/// </summary>
		/// <seealso cref="FrameAwaiter"/>
		public struct FrameAwaitable
		{
			private readonly FrameAwaiter _awaiter;

			/// <summary>
			/// Initializes a new instance of the <see cref="FrameAwaitable"/> struct.
			/// </summary>
			public FrameAwaitable(FrameTiming frameTime)
			{
				_awaiter = new FrameAwaiter(frameTime);
			}

			/// <summary>
			/// Returns the awaiter.
			/// </summary>
			public FrameAwaiter GetAwaiter()
			{
				return _awaiter;
			}
		}

		/// <summary>
		/// Waits until the specified <paramref name="frameTime"/>.
		/// </summary>
		/// <param name="frameTime">The frame time to await.</param>
		public static FrameAwaitable FrameUpdate(FrameTiming frameTime = FrameTiming.Update)
		{
			return new FrameAwaitable(frameTime);
		}

#endif

		/// <summary>
		/// Initializes utilities. This method is intended for internal use only. DO NOT use.
		/// </summary>
		internal static void Initialize(GameObject go, SynchronizationContext mainThreadContext)
		{
			_mainThreadContext = mainThreadContext;
			_rootBehaviour = go.AddComponent<AsyncUtilityBehaviour>();
		}

		#endregion

		#region implementation

		private sealed class AsyncUtilityBehaviour : MonoBehaviour
		{
			#region data

			private Dictionary<object, Action> _ops;
			private List<KeyValuePair<object, Action>> _opsToRemove;

			private AsyncUpdateSource _updateSource;
			private AsyncUpdateSource _lateUpdateSource;
			private AsyncUpdateSource _fixedUpdateSource;
			private AsyncUpdateSource _eofUpdateSource;
			private WaitForEndOfFrame _eof;

#if NET_4_6 || NET_STANDARD_2_0

			private ConcurrentQueue<Action> _updateActions;
			private ConcurrentQueue<Action> _lateUpdateActions;
			private ConcurrentQueue<Action> _fixedUpdateActions;
			private ConcurrentQueue<Action> _eofUpdateActions;

#else

			private Queue<Action> _updateActions;
			private Queue<Action> _lateUpdateActions;
			private Queue<Action> _fixedUpdateActions;
			private Queue<Action> _eofUpdateActions;

#endif

			#endregion

			#region interface

			public IAsyncUpdateSource UpdateSource
			{
				get
				{
					if (_updateSource == null)
					{
						_updateSource = new AsyncUpdateSource();
					}

					return _updateSource;
				}
			}

			public IAsyncUpdateSource LateUpdateSource
			{
				get
				{
					if (_lateUpdateSource == null)
					{
						_lateUpdateSource = new AsyncUpdateSource();
					}

					return _lateUpdateSource;
				}
			}

			public IAsyncUpdateSource FixedUpdateSource
			{
				get
				{
					if (_fixedUpdateSource == null)
					{
						_fixedUpdateSource = new AsyncUpdateSource();
					}

					return _fixedUpdateSource;
				}
			}

			public IAsyncUpdateSource EofUpdateSource
			{
				get
				{
					if (_eofUpdateSource == null)
					{
						_eofUpdateSource = new AsyncUpdateSource();
						_eof = new WaitForEndOfFrame();
						StartCoroutine(EofEnumerator());
					}

					return _eofUpdateSource;
				}
			}

			public void AddCompletionCallback(object op, Action cb)
			{
				if (_ops == null)
				{
					_ops = new Dictionary<object, Action>();
					_opsToRemove = new List<KeyValuePair<object, Action>>();
				}

				_ops.Add(op, cb);
			}

			public void AddFrameCallback(Action callback, FrameTiming timing)
			{
				switch (timing)
				{
					case FrameTiming.FixedUpdate:
						AddFrameCallback(ref _fixedUpdateActions, callback);
						break;

					case FrameTiming.Update:
						AddFrameCallback(ref _updateActions, callback);
						break;

					case FrameTiming.LateUpdate:
						AddFrameCallback(ref _lateUpdateActions, callback);
						break;

					case FrameTiming.EndOfFrame:
						AddFrameCallback(ref _eofUpdateActions, callback);
						break;
				}
			}

			#endregion

			#region MonoBehavoiur

			private void Update()
			{
				if (_ops != null && _ops.Count > 0)
				{
					foreach (var item in _ops)
					{
						if (item.Key is AsyncOperation)
						{
							var asyncOp = item.Key as AsyncOperation;

							if (asyncOp.isDone)
							{
								_opsToRemove.Add(item);
							}
						}
						else if (item.Key is UnityWebRequest)
						{
							var asyncOp = item.Key as UnityWebRequest;

							if (asyncOp.isDone)
							{
								_opsToRemove.Add(item);
							}
						}
#if !UNITY_2018_3_OR_NEWER
						else if (item.Key is WWW)
						{
							var asyncOp = item.Key as WWW;

							if (asyncOp.isDone)
							{
								_opsToRemove.Add(item);
							}
						}
#endif
					}

					foreach (var item in _opsToRemove)
					{
						_ops.Remove(item.Key);

						try
						{
							item.Value();
						}
						catch (Exception e)
						{
							Debug.LogException(e, this);
						}
					}

					_opsToRemove.Clear();
				}

				if (_updateSource != null)
				{
					try
					{
						_updateSource.OnNext(Time.deltaTime);
					}
					catch (Exception e)
					{
						Debug.LogException(e, this);
					}
				}

				if (_updateActions != null)
				{
					InvokeFrameCallbacks(_updateActions, this);
				}

				if (_mainThreadContext is Helpers.MainThreadSynchronizationContext)
				{
					(_mainThreadContext as Helpers.MainThreadSynchronizationContext).Update(this);
				}
			}

			private void LateUpdate()
			{
				if (_lateUpdateSource != null)
				{
					try
					{
						_lateUpdateSource.OnNext(Time.deltaTime);
					}
					catch (Exception e)
					{
						Debug.LogException(e, this);
					}
				}

				if (_lateUpdateActions != null)
				{
					InvokeFrameCallbacks(_lateUpdateActions, this);
				}
			}

			private void FixedUpdate()
			{
				if (_fixedUpdateSource != null)
				{
					try
					{
						_fixedUpdateSource.OnNext(Time.fixedDeltaTime);
					}
					catch (Exception e)
					{
						Debug.LogException(e, this);
					}
				}

				if (_fixedUpdateActions != null)
				{
					InvokeFrameCallbacks(_fixedUpdateActions, this);
				}
			}

			private void OnDestroy()
			{
				if (_updateSource != null)
				{
					_updateSource.Dispose();
					_updateSource = null;
				}

				if (_lateUpdateSource != null)
				{
					_lateUpdateSource.Dispose();
					_lateUpdateSource = null;
				}

				if (_fixedUpdateSource != null)
				{
					_fixedUpdateSource.Dispose();
					_fixedUpdateSource = null;
				}

				if (_eofUpdateSource != null)
				{
					_eofUpdateSource.Dispose();
					_eofUpdateSource = null;
				}
			}

			#endregion

			#region implementation

#if NET_4_6 || NET_STANDARD_2_0

			private static void AddFrameCallback(ref ConcurrentQueue<Action> actionQueue, Action callback)
			{
				Interlocked.CompareExchange(ref actionQueue, new ConcurrentQueue<Action>(), null);
				actionQueue.Enqueue(callback);
			}

			private static void InvokeFrameCallbacks(ConcurrentQueue<Action> actionQueue, UnityEngine.Object context)
			{
				Action action;

				while (actionQueue.TryDequeue(out action))
				{
					try
					{
						action.Invoke();
					}
					catch (Exception e)
					{
						Debug.LogException(e, context);
					}
				}
			}

#else

			private void AddFrameCallback(ref Queue<Action> actionQueue, Action callback)
			{
				Interlocked.CompareExchange(ref actionQueue, new Queue<Action>(), null);

				lock (actionQueue)
				{
					actionQueue.Enqueue(callback);
				}
			}

			private static void InvokeFrameCallbacks(Queue<Action> actionQueue, UnityEngine.Object context)
			{
				if (actionQueue.Count > 0)
				{
					lock (actionQueue)
					{
						while (actionQueue.Count > 0)
						{
							try
							{
								actionQueue.Dequeue().Invoke();
							}
							catch (Exception e)
							{
								Debug.LogException(e, context);
							}
						}
					}
				}
			}

#endif

			private IEnumerator EofEnumerator()
			{
				yield return _eof;

				if (_eofUpdateSource != null)
				{
					try
					{
						_eofUpdateSource.OnNext(Time.deltaTime);
					}
					catch (Exception e)
					{
						Debug.LogException(e, this);
					}
				}

				if (_eofUpdateActions != null)
				{
					InvokeFrameCallbacks(_eofUpdateActions, this);
				}
			}

			#endregion
		}

		#endregion
	}
}
