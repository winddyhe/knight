// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
	using Debug = System.Diagnostics.Debug;

	/// <summary>
	/// A wrapper for <see cref="AssetBundleRequest"/> with result value.
	/// </summary>
	/// <typeparam name="T">Result type.</typeparam>
	internal class AssetBundleRequestResult<T> : AsyncOperationResult<T> where T : UnityEngine.Object
	{
		#region data

		private readonly string _assetName;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		protected AssetBundleRequestResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="assetName">Name of an asset to load.</param>
		public AssetBundleRequestResult(string assetName)
		{
			_assetName = assetName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="assetName">Name of an asset to load.</param>
		/// <param name="userState">User-defined data.</param>
		public AssetBundleRequestResult(string assetName, object userState)
			: base(null, userState)
		{
			_assetName = assetName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="assetbundle">The asset bundle to load asset from.</param>
		/// <param name="assetName">Name of an asset to load.</param>
		public AssetBundleRequestResult(AssetBundle assetbundle, string assetName)
			: base(string.IsNullOrEmpty(assetName) ? assetbundle.LoadAllAssetsAsync() : assetbundle.LoadAssetAsync(assetName))
		{
			_assetName = assetName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="assetbundle">The asset bundle to load asset from.</param>
		/// <param name="assetName">Name of an asset to load.</param>
		/// <param name="userState">User-defined data.</param>
		public AssetBundleRequestResult(AssetBundle assetbundle, string assetName, object userState)
			: base(string.IsNullOrEmpty(assetName) ? assetbundle.LoadAllAssetsAsync() : assetbundle.LoadAssetAsync(assetName), userState)
		{
			_assetName = assetName;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source operation.</param>
		public AssetBundleRequestResult(AssetBundleRequest op)
			: base(op)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleRequestResult{T}"/> class.
		/// </summary>
		/// <param name="op">Source operation.</param>
		/// <param name="userState">User-defined data.</param>
		public AssetBundleRequestResult(AssetBundleRequest op, object userState)
			: base(op, userState)
		{
		}

		#endregion

		#region AsyncOperationResult

		/// <inheritdoc/>
		protected override T GetResult(AsyncOperation op)
		{
			var abr = op as AssetBundleRequest;

			if (abr.asset != null)
			{
				return abr.asset as T;
			}
			else if (abr.allAssets != null)
			{
				foreach (var asset in abr.allAssets)
				{
					if (asset is T)
					{
						return asset as T;
					}
				}
			}

			return null;
		}

		#endregion

		#region IAsyncContinuation

		/// <inheritdoc/>
		public override void Invoke(IAsyncOperation op)
		{
			var abr = op as IAsyncOperation<AssetBundle>;

			if (abr != null && abr.IsCompletedSuccessfully)
			{
				Debug.Assert(Operation == null);

				if (string.IsNullOrEmpty(_assetName))
				{
					Operation = abr.Result.LoadAllAssetsAsync();
				}
				else
				{
					Operation = abr.Result.LoadAssetAsync(_assetName);
				}

				Start();
			}
			else
			{
				base.Invoke(op);
			}
		}

		#endregion
	}
}
