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
	internal class PlayAnimatorResult : AsyncResult, IAsyncUpdatable
	{
		#region data

		private readonly IAsyncUpdateSource _updateSource;
		private readonly Animator _anim;
		private readonly int _stateNameHash;
		private readonly int _layer;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PlayAnimatorResult"/> class.
		/// </summary>
		public PlayAnimatorResult(Animator anim, int stateNameHash, int layer, IAsyncUpdateSource updateSource)
		{
			Debug.Assert(anim != null);
			Debug.Assert(updateSource != null);

			_updateSource = updateSource;
			_anim = anim;
			_stateNameHash = stateNameHash;
			_layer = layer;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PlayAnimatorResult"/> class.
		/// </summary>
		public PlayAnimatorResult(Animator anim, string stateName, int layer, IAsyncUpdateSource updateSource)
		{
			Debug.Assert(anim != null);
			Debug.Assert(stateName != null);
			Debug.Assert(updateSource != null);

			_updateSource = updateSource;
			_anim = anim;
			_stateNameHash = Animator.StringToHash(stateName);
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
			_anim.Play(_stateNameHash, _layer);
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
