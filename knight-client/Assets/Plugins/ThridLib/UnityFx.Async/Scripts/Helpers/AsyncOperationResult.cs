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
	internal class AsyncOperationResult : AsyncResult
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
		/// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
		/// </summary>
		protected AsyncOperationResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		public AsyncOperationResult(AsyncOperation op)
		{
			_op = op;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncOperationResult"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		public AsyncOperationResult(AsyncOperation op, object userState)
			: base(null, userState)
		{
			_op = op;
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			Debug.Assert(_op != null);

			if (_op.isDone)
			{
				TrySetCompleted(true);
			}
			else
			{
#if UNITY_2017_2_OR_NEWER

				// Starting with Unity 2017.2 there is AsyncOperation.completed event
				_op.completed += o => TrySetCompleted();

#else

				AsyncUtility.AddCompletionCallback(_op, () => TrySetCompleted());

#endif
			}
		}

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			return _op.progress;
		}

		#endregion
	}
}
