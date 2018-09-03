// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using UnityEngine;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif

namespace UnityFx.Async
{
	/// <summary>
	/// Unity web request utilities.
	/// </summary>
	public static class AsyncWww
	{
		/// <summary>
		/// Creates an asyncronous operation optimized for downloading text via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the text to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<string> GetText(string url)
		{
#if UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.Get(url);
			var result = new Helpers.WebRequestResult<string>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<string>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading binary content via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the binary content to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<byte[]> GetBytes(string url)
		{
#if UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.Get(url);
			var result = new Helpers.WebRequestResult<byte[]>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<byte[]>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<AssetBundle> GetAssetBundle(string url)
		{
#if UNITY_2018_1_OR_NEWER

			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetAssetBundle(url);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<AssetBundle>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<AssetBundle> GetAssetBundle(string url, Hash128 hash)
		{
#if UNITY_2018_1_OR_NEWER

			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, 0);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetAssetBundle(url, hash, 0);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#else

			var www = WWW.LoadFromCacheOrDownload(url, hash);
			var result = new Helpers.WwwResult<AssetBundle>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AssetBundle"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the asset bundle to download.</param>
		/// <param name="hash">A version hash. If this hash does not match the hash for the cached version of this asset bundle, the asset bundle will be redownloaded.</param>
		/// <param name="crc">If nonzero, this number will be compared to the checksum of the downloaded asset bundle data. If the CRCs do not match, an error will be logged and the asset bundle will not be loaded. If set to zero, CRC checking will be skipped.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<AssetBundle> GetAssetBundle(string url, Hash128 hash, uint crc)
		{
#if UNITY_2018_1_OR_NEWER

			var webRequest = UnityWebRequestAssetBundle.GetAssetBundle(url, hash, crc);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetAssetBundle(url, hash, crc);
			var result = new Helpers.WebRequestResult<AssetBundle>(webRequest);

#else

			var www = WWW.LoadFromCacheOrDownload(url, hash, crc);
			var result = new Helpers.WwwResult<AssetBundle>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AudioClip"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the audio clip to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<AudioClip> GetAudioClip(string url)
		{
#if UNITY_2017_1_OR_NEWER

			var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN);
			var result = new Helpers.WebRequestResult<AudioClip>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetAudioClip(url, AudioType.UNKNOWN);
			var result = new Helpers.WebRequestResult<AudioClip>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<AudioClip>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="AudioClip"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the audio clip to download.</param>
		/// <param name="audioType">The type of audio encoding for the downloaded audio clip.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<AudioClip> GetAudioClip(string url, AudioType audioType)
		{
#if UNITY_2017_1_OR_NEWER

			var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, audioType);
			var result = new Helpers.WebRequestResult<AudioClip>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetAudioClip(url, audioType);
			var result = new Helpers.WebRequestResult<AudioClip>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<AudioClip>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="Texture2D"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the texture to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<Texture2D> GetTexture(string url)
		{
#if UNITY_2017_1_OR_NEWER

			var webRequest = UnityWebRequestTexture.GetTexture(url, false);
			var result = new Helpers.WebRequestResult<Texture2D>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetTexture(url);
			var result = new Helpers.WebRequestResult<Texture2D>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<Texture2D>(www);

#endif

			result.Start();
			return result;
		}

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="Texture2D"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the texture to download.</param>
		/// <param name="nonReadable">If <see langword="true"/>, the texture's raw data will not be accessible to script. This can conserve memory.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<Texture2D> GetTexture(string url, bool nonReadable)
		{
#if UNITY_2017_1_OR_NEWER

			var webRequest = UnityWebRequestTexture.GetTexture(url, nonReadable);
			var result = new Helpers.WebRequestResult<Texture2D>(webRequest);

#elif UNITY_5_4_OR_NEWER

			var webRequest = UnityWebRequest.GetTexture(url, nonReadable);
			var result = new Helpers.WebRequestResult<Texture2D>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<Texture2D>(www);

#endif

			result.Start();
			return result;
		}

#if !UNITY_2018_2_OR_NEWER

		/// <summary>
		/// Creates an asyncronous operation optimized for downloading a <see cref="MovieTexture"/> via HTTP GET.
		/// </summary>
		/// <param name="url">The URI of the texture to download.</param>
		/// <returns>An operation that can be used to track the download process.</returns>
		public static IAsyncOperation<MovieTexture> GetMovieTexture(string url)
		{
#if UNITY_2017_1_OR_NEWER

			var webRequest = UnityWebRequestMultimedia.GetMovieTexture(url);
			var result = new Helpers.WebRequestResult<MovieTexture>(webRequest);

#else

			var www = new WWW(url);
			var result = new Helpers.WwwResult<MovieTexture>(www);

#endif

			result.Start();
			return result;
		}

#endif

#if UNITY_5_4_OR_NEWER

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
#if !UNITY_5 && !UNITY_2018_2_OR_NEWER
			else if (request.downloadHandler is DownloadHandlerMovieTexture)
			{
				return ((DownloadHandlerMovieTexture)request.downloadHandler).movieTexture as T;
			}
#endif
			else if (typeof(T) == typeof(byte[]))
			{
				return request.downloadHandler.data as T;
			}
			else if (typeof(T) != typeof(object))
			{
				return request.downloadHandler.text as T;
			}

			return default(T);
		}

#endif

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
			else if (typeof(T) == typeof(AudioClip))
			{
#if UNITY_5_4_OR_NEWER
				return request.GetAudioClip() as T;
#else
				return request.audioClip as T;
#endif
			}
#if !UNITY_2018_2_OR_NEWER
			else if (typeof(T) == typeof(MovieTexture))
			{
#if UNITY_5_4_OR_NEWER
				return request.GetMovieTexture() as T;
#else
				return request.movie as T;
#endif
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
	}
}
