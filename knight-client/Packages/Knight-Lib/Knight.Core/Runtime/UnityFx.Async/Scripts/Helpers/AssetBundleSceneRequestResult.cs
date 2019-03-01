// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityFx.Async.Helpers
{
	/// <summary>
	/// A wrapper for <see cref="AsyncOperation"/> with <see cref="Scene"/> result value.
	/// </summary>
	internal class AssetBundleSceneRequestResult : AsyncOperationResult<Scene>
	{
		#region data

		private readonly LoadSceneMode _loadMode;
		private string _sceneName;

		#endregion

		#region interface

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleSceneRequestResult"/> class.
		/// </summary>
		protected AssetBundleSceneRequestResult()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleSceneRequestResult"/> class.
		/// </summary>
		public AssetBundleSceneRequestResult(string sceneName, LoadSceneMode loadMode)
		{
			_sceneName = sceneName;
			_loadMode = loadMode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleSceneRequestResult"/> class.
		/// </summary>
		public AssetBundleSceneRequestResult(string sceneName, LoadSceneMode loadMode, object userState)
			: base(null, userState)
		{
			_sceneName = sceneName;
			_loadMode = loadMode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetBundleSceneRequestResult"/> class.
		/// </summary>
		public AssetBundleSceneRequestResult(AssetBundle assetBundle, string sceneName, LoadSceneMode loadMode, object userState)
			: base(GetAsyncOperation(assetBundle, ref sceneName, loadMode), userState)
		{
			_sceneName = sceneName;
			_loadMode = loadMode;
		}

		#endregion

		#region AsyncOperationResult

		/// <inheritdoc/>
		protected override Scene GetResult(AsyncOperation op)
		{
			var scene = default(Scene);

			// NOTE: Grab the last scene with the specified name from the list of loaded scenes.
			for (var i = SceneManager.sceneCount - 1; i >= 0; --i)
			{
				var s = SceneManager.GetSceneAt(i);

				if (s.name == _sceneName)
				{
					scene = s;
					break;
				}
			}

			if (!scene.isLoaded)
			{
				throw new AssetLoadException(_sceneName, typeof(Scene));
			}

			return scene;
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

				Operation = GetAsyncOperation(abr.Result, ref _sceneName, _loadMode);
				Start();
			}
			else
			{
				base.Invoke(op);
			}
		}

		#endregion

		#region implementation

		private static AsyncOperation GetAsyncOperation(AssetBundle assetBundle, ref string sceneName, LoadSceneMode loadMode)
		{
			if (assetBundle.isStreamedSceneAssetBundle)
			{
				if (string.IsNullOrEmpty(sceneName))
				{
					var scenePaths = assetBundle.GetAllScenePaths();

					if (scenePaths != null && scenePaths.Length > 0 && !string.IsNullOrEmpty(scenePaths[0]))
					{
						sceneName = Path.GetFileNameWithoutExtension(scenePaths[0]);
					}
				}

				if (string.IsNullOrEmpty(sceneName))
				{
					throw new AssetLoadException("The asset bundle does not contain scenes.", null, typeof(Scene));
				}
				else
				{
					return SceneManager.LoadSceneAsync(sceneName, loadMode);
				}
			}
			else
			{
				throw new AssetLoadException("The asset bundle does not contain scenes.", null, typeof(Scene));
			}
		}

		#endregion
	}
}
