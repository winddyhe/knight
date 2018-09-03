// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	using Debug = System.Diagnostics.Debug;

	/// <summary>
	/// A wrapper for <see cref="Animator"/>.
	/// </summary>
	public class WaitAnimatorResult : AsyncResult, IAsyncUpdatable
	{
		#region data

		private readonly IAsyncUpdateSource _updateSource;
		private readonly Animator _anim;
		private readonly int _layer;

		private int _stateNameHash;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="WaitAnimatorResult"/> class.
		/// </summary>
		public WaitAnimatorResult(Animator anim, int layer, IAsyncUpdateSource updateSource)
		{
			Debug.Assert(anim != null);
			Debug.Assert(updateSource != null);

			_updateSource = updateSource;
			_anim = anim;
			_layer = layer;
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			var state = _anim.GetCurrentAnimatorStateInfo(_layer);

			if (!state.loop && state.shortNameHash == _stateNameHash)
			{
				return state.normalizedTime;
			}

			return base.GetProgress();
		}

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			_stateNameHash = _anim.GetCurrentAnimatorStateInfo(_layer).shortNameHash;
			_updateSource.AddListener(this);
		}

		/// <inheritdoc/>
		protected override void OnCompleted()
		{
			_updateSource.RemoveListener(this);
			base.OnCompleted();
		}

		#endregion

		#region IAsyncUpdatable

		/// <inheritdoc/>
		public void Update(float frameTime)
		{
			if (_anim.isActiveAndEnabled)
			{
				var state = _anim.GetCurrentAnimatorStateInfo(_layer);

				if (state.shortNameHash != _stateNameHash || state.normalizedTime >= 1)
				{
					TrySetCompleted();
				}
			}
			else
			{
				TrySetCanceled();
			}
		}

		#endregion
	}
}
