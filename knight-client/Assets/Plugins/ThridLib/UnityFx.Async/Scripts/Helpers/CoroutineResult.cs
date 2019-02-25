// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Collections;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	/// <summary>
	/// A wrapper for a <see cref="Coroutine"/>.
	/// </summary>
	internal class CoroutineResult : AsyncCompletionSource
	{
		#region data

		private readonly MonoBehaviour _coroutineRunner;
		private readonly IEnumerator _coroutineEnum;
		private Coroutine _coroutine;

		#endregion

		#region interface

		public CoroutineResult(MonoBehaviour monoBehaviour, Func<IAsyncCompletionSource, IEnumerator> coroutineFunc, object userState)
			: base(AsyncOperationStatus.Running, userState)
		{
			_coroutineRunner = monoBehaviour;
			_coroutineEnum = coroutineFunc(this);

			if (_coroutineEnum == null)
			{
				new InvalidOperationException("Cannot start a coroutine from null enumerator.");
			}
		}

		#endregion

		#region AsyncResult

		protected override void OnStarted()
		{
			_coroutine = _coroutineRunner.StartCoroutine(CoroutineWrapperEnum());
		}

		protected override void OnCompleted()
		{
			_coroutine = null;
		}

		protected override void OnCancel()
		{
			if (_coroutine != null)
			{
				_coroutineRunner.StopCoroutine(_coroutine);
			}

			TrySetCanceled(false);
		}

		#endregion

		#region implementation

		private IEnumerator CoroutineWrapperEnum()
		{
			yield return _coroutineEnum;
			TrySetCompleted(false);
		}

		#endregion
	}
}
