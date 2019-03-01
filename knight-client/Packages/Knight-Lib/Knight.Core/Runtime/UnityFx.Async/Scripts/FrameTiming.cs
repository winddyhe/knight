// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;

namespace UnityFx.Async
{
	/// <summary>
	/// Enumerates time points during a frame.
	/// </summary>
	public enum FrameTiming
	{
		/// <summary>
		/// FixedUpdate.
		/// </summary>
		FixedUpdate,

		/// <summary>
		/// Update.
		/// </summary>
		Update,

		/// <summary>
		/// LateUpdate.
		/// </summary>
		LateUpdate,

		/// <summary>
		/// yield WaitForEndOfFrame().
		/// </summary>
		EndOfFrame
	}
}
