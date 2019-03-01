// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UnityFx.Async.CompilerServices
{
#if NET_4_6 || NET_STANDARD_2_0

	/// <summary>
	/// Provides an awaitable object that allows for configured awaits on <see cref="YieldInstruction"/>.
	/// This type is intended for compiler use only.
	/// </summary>
	/// <seealso cref="YieldInstruction"/>
	public class YieldInstructionAwaiter : IEnumerator, INotifyCompletion
	{
		#region data

		private YieldInstruction _yieldValue;
		private Action _callback;
		private object _current;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="YieldInstructionAwaiter"/> class.
		/// </summary>
		public YieldInstructionAwaiter(YieldInstruction op)
		{
			_yieldValue = op;
		}

		/// <summary>
		/// Gets a value indicating whether the underlying operation is completed.
		/// </summary>
		/// <value>The operation completion flag.</value>
		public bool IsCompleted
		{
			get
			{
				return _current == null && _yieldValue == null;
			}
		}

		/// <summary>
		/// Returns the source result value.
		/// </summary>
		public void GetResult()
		{
		}

		#endregion

		#region INotifyCompletion

		/// <inheritdoc/>
		public void OnCompleted(Action continuation)
		{
			_callback = continuation;
			AsyncUtility.StartCoroutine(this);
		}

		#endregion

		#region IEnumerator

		/// <inheritdoc/>
		public object Current
		{
			get
			{
				return _current;
			}
		}

		/// <inheritdoc/>
		public bool MoveNext()
		{
			if (_yieldValue != null)
			{
				if (_current == null)
				{
					_current = _yieldValue;
					return true;
				}
				else
				{
					_current = null;
					_yieldValue = null;
					_callback();
				}
			}

			return false;
		}

		/// <inheritdoc/>
		public void Reset()
		{
			throw new NotSupportedException();
		}

		#endregion
	}

#endif
}
