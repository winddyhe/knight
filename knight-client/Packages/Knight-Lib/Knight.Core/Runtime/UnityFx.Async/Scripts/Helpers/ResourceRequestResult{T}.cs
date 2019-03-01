// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	/// <summary>
	/// A wrapper for <see cref="ResourceRequest"/> with result value.
	/// </summary>
	/// <typeparam name="T">Result type.</typeparam>
	internal class ResourceRequestResult<T> : AsyncOperationResult<T> where T : UnityEngine.Object
	{
		#region data
		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceRequestResult{T}"/> class.
		/// </summary>
		protected ResourceRequestResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceRequestResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		public ResourceRequestResult(ResourceRequest op)
			: base(op)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceRequestResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		public ResourceRequestResult(ResourceRequest op, object userState)
			: base(op, userState)
		{
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override T GetResult(AsyncOperation op)
		{
			return (op as ResourceRequest).asset as T;
		}

		#endregion
	}
}
