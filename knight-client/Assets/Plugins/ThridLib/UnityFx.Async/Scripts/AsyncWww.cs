// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace UnityFx.Async
{
	/// <summary>
	/// Unity web request utilities.
	/// </summary>
	public static class AsyncWww
	{
		#region interface

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading text via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the text to download.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetBytesAsync(string, object)"/>
		public static IAsyncOperation<string> GetTextAsync(string url, object userState = null)
		{
			ThrowIfInvalidUrl(url);

			var webRequest = UnityWebRequest.Get(url);
			var result = new Helpers.WebRequestResult<string>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading binary content via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the binary content to download.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetTextAsync(string, object)"/>
		public static IAsyncOperation<byte[]> GetBytesAsync(string url, object userState = null)
		{
			ThrowIfInvalidUrl(url);

			var webRequest = UnityWebRequest.Get(url);
			var result = new Helpers.WebRequestResult<byte[]>(webRequest);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleAsync(string, Hash128, uint, object)"/>
		public static IAsyncOperation<AssetBundle> GetAssetBundleAsync(string url, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url);
#endif

			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleAsync(string, object)"/>
		public static IAsyncOperation<AssetBundle> GetAssetBundleAsync(string url, Hash128 hash, uint crc, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url, hash, crc);
#endif

			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading assets from <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="assetName">Name of the prefab to load. If <see langword="null"/> the first asset of the matching type is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleAssetAsync(string, Hash128, uint, string, object)"/>
		public static IAsyncOperation<T> GetAssetBundleAssetAsync<T>(string url, string assetName, object userState = null) where T : UnityEngine.Object
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url);
#endif

			var result = new Helpers.AssetBundleLoadAssetResult<T>(webRequest, assetName, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading assets from <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="assetName">Name of the prefab to load. If <see langword="null"/> the first asset of the matching type is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleAssetAsync(string, string, object)"/>
		public static IAsyncOperation<T> GetAssetBundleAssetAsync<T>(string url, Hash128 hash, uint crc, string assetName, object userState = null) where T : UnityEngine.Object
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url, hash, crc);
#endif

			var result = new Helpers.AssetBundleLoadAssetResult<T>(webRequest, assetName, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for loading assets from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <param name="assetBundle">The source <see cref="AssetBundle"/>.</param>
		/// <param name="assetName">Name of the prefab to load. If <see langword="null"/> the first asset of the matching type is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetBundle"/> is <see langword="null"/>.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<T> GetAssetBundleAssetAsync<T>(AssetBundle assetBundle, string assetName, object userState = null) where T : UnityEngine.Object
		{
			if (assetBundle == null)
			{
				throw new ArgumentNullException("assetBundle");
			}

			var result = new Helpers.AssetBundleRequestResult<T>(assetBundle, assetName, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading prefabs from an <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="prefabName">Name of the prefab to load. If <see langword="null"/> the first prefab is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundlePrefabAsync(string, Hash128, uint, string, object)"/>
		public static IAsyncOperation<GameObject> GetAssetBundlePrefabAsync(string url, string prefabName, object userState = null)
		{
			return GetAssetBundleAssetAsync<GameObject>(url, prefabName, userState);
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading prefabs from <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="prefabName">Name of the prefab to load. If <see langword="null"/> the first prefab is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundlePrefabAsync(string, string, object)"/>
		public static IAsyncOperation<GameObject> GetAssetBundlePrefabAsync(string url, Hash128 hash, uint crc, string prefabName, object userState = null)
		{
			return GetAssetBundleAssetAsync<GameObject>(url, hash, crc, prefabName, userState);
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for loading prefabs from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <param name="assetBundle">The source <see cref="AssetBundle"/>.</param>
		/// <param name="prefabName">Name of the prefab to load. If <see langword="null"/> the first prefab is loaded.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetBundle"/> is <see langword="null"/>.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundlePrefabAsync(string, Hash128, uint, string, object)"/>
		public static IAsyncOperation<GameObject> GetAssetBundlePrefabAsync(AssetBundle assetBundle, string prefabName, object userState = null)
		{
			return GetAssetBundleAssetAsync<GameObject>(assetBundle, prefabName, userState);
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading scenes from <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="sceneName">Name of the prefab to load.</param>
		/// <param name="loadMode">Scene load mode.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleSceneAsync(string, Hash128, uint, string, LoadSceneMode, object)"/>
		public static IAsyncOperation<Scene> GetAssetBundleSceneAsync(string url, string sceneName, LoadSceneMode loadMode, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url);
#endif

			var result = new Helpers.AssetBundleLoadSceneResult(webRequest, sceneName, loadMode, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading scenes from <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <param name="sceneName">Name of the prefab to load.</param>
		/// <param name="loadMode">Scene load mode.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAssetBundleSceneAsync(string, string, LoadSceneMode, object)"/>
		public static IAsyncOperation<Scene> GetAssetBundleSceneAsync(string url, Hash128 hash, uint crc, string sceneName, LoadSceneMode loadMode, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2018_1_OR_NEWER
			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc);
#else
			var webRequest = UnityWebRequest.GetAssetBundle(url, hash, crc);
#endif

			var result = new Helpers.AssetBundleLoadSceneResult(webRequest, sceneName, loadMode, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for loading scenes from an <see cref="AssetBundle"/>.
		/// </summary>
		/// <param name="assetBundle">The source <see cref="AssetBundle"/>.</param>
		/// <param name="sceneName">Name of the scene to load. If <see langword="null"/> the first scene in the asset bundle is loaded.</param>
		/// <param name="loadMode">Scene load mode.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="assetBundle"/> is <see langword="null"/>.</exception>
		/// <returns>An operation that can be used to track the load process.</returns>
		public static IAsyncOperation<Scene> GetAssetBundleSceneAsync(AssetBundle assetBundle, string sceneName, LoadSceneMode loadMode, object userState = null)
		{
			if (assetBundle == null)
			{
				throw new ArgumentNullException("assetBundle");
			}

			var result = new Helpers.AssetBundleSceneRequestResult(assetBundle, sceneName, loadMode, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AudioClip"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the audio clip to download.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAudioClipAsync(string, AudioType, object)"/>
		public static IAsyncOperation<AudioClip> GetAudioClipAsync(string url, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2017_1_OR_NEWER
			var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
#else
			var webRequest = UnityWebRequest.GetAudioClip(url, AudioType.UNKNOWN);
#endif

			var result = new Helpers.WebRequestResult<AudioClip>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AudioClip"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the audio clip to download.</param>
		/// <param name="audioType">The type of audio encoding for the downloaded audio clip.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetAudioClipAsync(string, object)"/>
		public static IAsyncOperation<AudioClip> GetAudioClipAsync(string url, AudioType audioType, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2017_1_OR_NEWER
			var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
#else
			var webRequest = UnityWebRequest.GetAudioClip(url, audioType);
#endif

			var result = new Helpers.WebRequestResult<AudioClip>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="Texture2D"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the texture to download.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetTextureAsync(string, bool, object)"/>
		public static IAsyncOperation<Texture2D> GetTextureAsync(string url, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2017_1_OR_NEWER
			var webRequest = UnityWebRequestTexture.GetTexture(url, false);
#else
			var webRequest = UnityWebRequest.GetTexture(url);
#endif

			var result = new Helpers.WebRequestResult<Texture2D>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="Texture2D"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the texture to download.</param>
		/// <param name="nonReadable">If <see langword="true"/>, the texture's raw data will not be accessible to script. This can conserve memory.</param>
		/// <param name="userState">User-defined data.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="url"/> is an empty string.</exception>
		/// <returns>An operation that can be used to track the download process.</returns>
		/// <seealso cref="GetTextureAsync(string, object)"/>
		public static IAsyncOperation<Texture2D> GetTextureAsync(string url, bool nonReadable, object userState = null)
		{
			ThrowIfInvalidUrl(url);

#if UNITY_2017_1_OR_NEWER
			var webRequest = UnityWebRequestTexture.GetTexture(url, nonReadable);
#else
			var webRequest = UnityWebRequest.GetTexture(url, nonReadable);
#endif

			var result = new Helpers.WebRequestResult<Texture2D>(webRequest, userState);
			result.Start();
			return result;
		}

		/// <summary>
		/// Returns result value of the specified <see cref="UnityWebRequest"/> instance.
		/// </summary>
		/// <param name="request">The request to get result for.</param>
		public static T GetResult<T>(UnityWebRequest request) where T : class
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (request.downloadHandler is DownloadHandlerAssetBundle)
			{
				return ((DownloadHandlerAssetBundle)request.downloadHandler).assetBundle as T;
			}
			else if (request.downloadHandler is DownloadHandlerTexture)
			{
				return ((DownloadHandlerTexture)request.downloadHandler).texture as T;
			}
			else if (request.downloadHandler is DownloadHandlerAudioClip)
			{
				return ((DownloadHandlerAudioClip)request.downloadHandler).audioClip as T;
			}
			else if (typeof(T) == typeof(byte[]))
			{
				return request.downloadHandler.data as T;
			}
			else if (typeof(T) != typeof(object))
			{
				return request.downloadHandler.text as T;
			}

			return null;
		}

#if !UNITY_2018_3_OR_NEWER

		/// <summary>
		/// Returns result value of the specified <see cref="WWW"/> instance.
		/// </summary>
		/// <param name="request">The request to get result for.</param>
		public static T GetResult<T>(WWW request) where T : class
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}

			if (typeof(T) == typeof(AssetBundle))
			{
				return request.assetBundle as T;
			}
			else if (typeof(T) == typeof(Texture2D))
			{
				return request.texture as T;
			}
#if UNITY_5_6_OR_NEWER
			else if (typeof(T) == typeof(AudioClip))
			{
				return request.GetAudioClip() as T;
			}
#else
			else if (typeof(T) == typeof(AudioClip))
			{
				return request.audioClip as T;
			}
#endif
			else if (typeof(T) == typeof(byte[]))
			{
				return request.bytes as T;
			}
			else if (typeof(T) != typeof(object))
			{
				return request.text as T;
			}

			return null;
		}

#endif

		/// <summary>
		/// Initializes WWW services. This method is intended for internal use only. DO NOT use.
		/// </summary>
		internal static void Initialize(GameObject go, SynchronizationContext mainThreadContext)
		{
			// TODO
		}

		#endregion

		#region implementation

		private static void ThrowIfInvalidUrl(string url)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}

#if NET_4_6 || NET_STANDARD_2_0
			if (string.IsNullOrWhiteSpace(url))
#else
			if (string.IsNullOrEmpty(url))
#endif
			{
				throw new ArgumentException("Invalid URL.", "url");
			}
		}

		#endregion
	}
}
