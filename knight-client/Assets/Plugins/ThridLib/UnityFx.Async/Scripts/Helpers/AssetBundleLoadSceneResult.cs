// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UnityFx.Async.Helpers
{
	internal class AssetBundleLoadSceneResult : AsyncResult<Scene>
	{
		#region data

		private readonly WebRequestResult<AssetBundle> _assetBundleLoadResult;
		private readonly AssetBundleSceneRequestResult _sceneLoadResult;

		#endregion

		#region interface

		public AssetBundleLoadSceneResult(UnityWebRequest request, string sceneName, LoadSceneMode loadMode, object userState)
			: base(null, userState)
		{
			_assetBundleLoadResult = new WebRequestResult<AssetBundle>(request);
			_sceneLoadResult = new AssetBundleSceneRequestResult(sceneName, loadMode);
			_assetBundleLoadResult.AddCompletionCallback(_sceneLoadResult);
			_sceneLoadResult.AddCompletionCallback(this);
		}

		#endregion

		#region AsyncResult

		protected override float GetProgress()
		{
			return (_assetBundleLoadResult.Progress + _sceneLoadResult.Progress) * 0.5f;
		}

		protected override void OnStarted()
		{
			_assetBundleLoadResult.Start();
		}

		protected override void OnCancel()
		{
			_assetBundleLoadResult.Cancel();
			_sceneLoadResult.Cancel();
			TrySetCanceled();
		}

		public override void Invoke(IAsyncOperation op)
		{
			_assetBundleLoadResult.Result.Unload(false);

			if (op.IsCompletedSuccessfully)
			{
				TrySetResult(_sceneLoadResult.Result);
			}
			else
			{
				TrySetException(op.Exception);
			}
		}

		#endregion

		#region implementation
		#endregion
	}
}
