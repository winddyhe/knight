// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
#if NET_4_6 || NET_STANDARD_2_0
using System.Threading.Tasks;
#endif

namespace UnityFx.Async.Extensions
{
	/// <summary>
	/// Extension methods for <see cref="YieldInstruction"/>.
	/// </summary>
	public static class YieldInstructionExtensions
	{
#if NET_4_6 || NET_STANDARD_2_0

		/// <summary>
		/// Returns the operation awaiter. This method is intended for compiler use only.
		/// </summary>
		/// <param name="op">The operation to await.</param>
		public static CompilerServices.YieldInstructionAwaiter GetAwaiter(this YieldInstruction op)
		{
			return new CompilerServices.YieldInstructionAwaiter(op);
		}

#endif
	}
}
