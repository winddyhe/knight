// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	using Debug = System.Diagnostics.Debug;

	/// <summary>
	/// A wrapper for <see cref="AsyncOperation"/> with result value.
	/// </summary>
	internal abstract class AsyncOperationResult<T> : AsyncResult<T>
	{
		#region data

		private AsyncOperation _op;

		#endregion

		#region interface

		/// <summary>
		/// Gets or sets the underlying <see cref="AsyncOperation"/> instance.
		/// </summary>
		public AsyncOperation Operation
		{
			get
			{
				return _op;
			}
			protected set
			{
				_op = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncOperationResult{T}"/> class.
		/// </summary>
		protected AsyncOperationResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncOperationResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		protected AsyncOperationResult(AsyncOperation op)
		{
			_op = op;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncOperationResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		protected AsyncOperationResult(AsyncOperation op, object userState)
			: base(null, userState)
		{
			_op = op;
		}

		/// <summary>
		/// Called when the source <see cref="AsyncOperation"/> is completed.
		/// </summary>
		protected abstract T GetResult(AsyncOperation op);

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			Debug.Assert(_op != null);

			if (_op.isDone)
			{
				OnSetCompleted(_op);
			}
			else
			{
#if UNITY_2017_2_OR_NEWER

				// Starting with Unity 2017.2 there is AsyncOperation.completed event
				_op.completed += OnSetCompleted;

#else

				AsyncUtility.AddCompletionCallback(_op, () => OnSetCompleted(_op));

#endif
			}
		}

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			if (_op == null)
			{
				return 0;
			}

			return _op.progress;
		}

		#endregion

		#region implementation

		private void OnSetCompleted(AsyncOperation op)
		{
			try
			{
				var result = GetResult(op);

				if (result != null)
				{
					TrySetResult(result);
				}
				else
				{
					throw new InvalidOperationException();
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
			finally
			{
				_op = null;
			}
		}

		#endregion
	}
}
