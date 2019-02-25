// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="Animator"/>.
	/// </summary>
	public static class AnimatorExtensions
	{
		/// <summary>
		/// Plays an animator state.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <param name="stateName">The state name.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animator anim, string stateName, int layer = -1)
		{
			var result = new Helpers.PlayAnimatorResult(anim, stateName, layer, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}

		/// <summary>
		/// Plays an animator state.
		/// </summary>
		/// <param name="anim">Target animator instance.</param>
		/// <param name="stateNameHash">Hash of the state name.</param>
		/// <param name="layer">The layer index. If layer is -1, it plays the first state with the given state name.</param>
		/// <returns>An asynchronous operation that can be used to track the animation progress.</returns>
		public static IAsyncOperation PlayAsync(this Animator anim, int stateNameHash, int layer = -1)
		{
			var result = new Helpers.PlayAnimatorResult(anim, stateNameHash, layer, AsyncUtility.GetUpdateSource());
			result.Start();
			return result;
		}
	}
}
