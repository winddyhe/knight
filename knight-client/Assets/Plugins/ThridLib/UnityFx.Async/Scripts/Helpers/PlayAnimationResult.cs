// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	using Debug = System.Diagnostics.Debug;

	/// <summary>
	/// A wrapper for <see cref="Animation"/>.
	/// </summary>
	public class PlayAnimationResult : AsyncResult, IAsyncUpdatable
	{
		#region data

		private readonly IAsyncUpdateSource _updateSource;
		private readonly Animation _anim;
		private readonly string _animationName;
		private readonly PlayMode _playMode;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="PlayAnimationResult"/> class.
		/// </summary>
		public PlayAnimationResult(Animation anim, string animationName, PlayMode mode, IAsyncUpdateSource updateSource)
		{
			Debug.Assert(anim != null);
			Debug.Assert(updateSource != null);

			_updateSource = updateSource;
			_anim = anim;
			_animationName = animationName;
			_playMode = mode;
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			if (_anim.Play(_animationName, _playMode))
			{
				_updateSource.AddListener(this);
			}
			else
			{
				TrySetCanceled();
			}
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
				if (!_anim.IsPlaying(_animationName))
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
