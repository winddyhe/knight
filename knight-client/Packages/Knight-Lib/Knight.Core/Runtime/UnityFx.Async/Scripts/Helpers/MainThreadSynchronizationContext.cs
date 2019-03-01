// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
#if NET_4_6 || NET_STANDARD_2_0
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
#endif
using System.Threading;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	/// <summary>
	/// Implementation of <see cref="SynchronizationContext"/> for Unity.
	/// </summary>
	internal sealed class MainThreadSynchronizationContext : SynchronizationContext
	{
		#region data

		private struct InvokeResult
		{
			private readonly SendOrPostCallback _callback;
			private readonly object _userState;

			public InvokeResult(SendOrPostCallback d, object userState)
			{
				_callback = d;
				_userState = userState;
			}

			public void Invoke(UnityEngine.Object context)
			{
				try
				{
					_callback.Invoke(_userState);
				}
				catch (Exception e)
				{
					Debug.LogException(e, context);
				}
			}
		}

#if NET_4_6 || NET_STANDARD_2_0
		private readonly ConcurrentQueue<InvokeResult> _actionQueue = new ConcurrentQueue<InvokeResult>();
#else
		private readonly Queue<InvokeResult> _actionQueue = new Queue<InvokeResult>();
#endif

		#endregion

		#region interface

		/// <summary>
		/// Calls all delegates scheduled to execute on the main thread.
		/// </summary>
		public void Update(UnityEngine.Object context)
		{
#if NET_4_6 || NET_STANDARD_2_0

			InvokeResult invokeResult;

			while (_actionQueue.TryDequeue(out invokeResult))
			{
				invokeResult.Invoke(context);
			}

#else

			if (_actionQueue.Count > 0)
			{
				lock (_actionQueue)
				{
					while (_actionQueue.Count > 0)
					{
						_actionQueue.Dequeue().Invoke(context);
					}
				}
			}

#endif
		}

		#endregion

		#region SynchronizationContext

		/// <inheritdoc/>
		public override SynchronizationContext CreateCopy()
		{
			return new MainThreadSynchronizationContext();
		}

		/// <inheritdoc/>
		public override void Send(SendOrPostCallback d, object state)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}

			if (this == Current)
			{
				d.Invoke(state);
			}
			else
			{
				var completed = false;
				var exception = default(Exception);
				var asyncResult = new InvokeResult(
					args =>
					{
						try
						{
							d.Invoke(args);
						}
						catch (Exception e)
						{
							exception = e;
						}
						finally
						{
							completed = true;
						}
					},
					state);

#if NET_4_6 || NET_STANDARD_2_0

				_actionQueue.Enqueue(asyncResult);

				var sw = new SpinWait();

				while (!completed)
				{
					sw.SpinOnce();
				}

				if (exception != null)
				{
					ExceptionDispatchInfo.Capture(exception).Throw();
				}

#else

				lock (_actionQueue)
				{
					_actionQueue.Enqueue(asyncResult);
				}

				while (!completed)
				{
					Thread.SpinWait(1);
				}

				if (exception != null)
				{
					throw exception;
				}

#endif
			}
		}

		/// <inheritdoc/>
		public override void Post(SendOrPostCallback d, object state)
		{
			if (d == null)
			{
				throw new ArgumentNullException("d");
			}

			var asyncResult = new InvokeResult(d, state);

#if NET_4_6 || NET_STANDARD_2_0

			_actionQueue.Enqueue(asyncResult);

#else

			lock (_actionQueue)
			{
				_actionQueue.Enqueue(asyncResult);
			}

#endif
		}

		#endregion
	}
}
