// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Async.CompilerServices
{
#if NET_4_6 || NET_STANDARD_2_0

	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="UnityWebRequest"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="UnityWebRequest"/>
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
		public bool IsCompleted
		{
			get
			{
				return _op.isDone;
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
			AsyncUtility.AddCompletionCallback(_op, continuation);
		}
	}

#endif
}
