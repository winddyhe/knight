// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace UnityFx.Async.Helpers
{
	/// <summary>
	/// A wrapper for <see cref="UnityWebRequest"/> with result value.
	/// </summary>
	/// <typeparam name="T">Type of the request result.</typeparam>
	internal class WebRequestResult<T> : AsyncResult<T> where T : class
	{
		#region data

		private readonly UnityWebRequest _request;
		private AsyncOperation _op;

		#endregion

		#region interface

		/// <summary>
		/// Gets the underlying <see cref="UnityWebRequest"/> instance.
		/// </summary>
		public UnityWebRequest WebRequest
		{
			get
			{
				return _request;
			}
		}

		/// <summary>
		/// Gets the request url string.
		/// </summary>
		public string Url
		{
			get
			{
				return _request.url;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		public WebRequestResult(UnityWebRequest request)
		{
			_request = request;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		public WebRequestResult(UnityWebRequest request, object userState)
			: base(null, userState)
		{
			_request = request;
		}

		/// <summary>
		/// Initializes the operation result value. Called when the underlying <see cref="UnityWebRequest"/> has completed withou errors.
		/// </summary>
		protected virtual T GetResult(UnityWebRequest request)
		{
			return AsyncWww.GetResult<T>(request);
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			if (_op != null)
			{
				return _op.progress;
			}

			return base.GetProgress();
		}

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			if (_request.isDone)
			{
				SetCompleted();
			}
			else if (_request.isModifiable)
			{
#if UNITY_2017_2_OR_NEWER
				_op = _request.SendWebRequest();
#else
				_op = _request.Send();
#endif

				AsyncUtility.AddCompletionCallback(_op, SetCompleted);
			}
		}

		/// <inheritdoc/>
		protected override void OnCancel()
		{
			if (TrySetCanceled(false))
			{
				_request.Abort();
			}
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_request.Dispose();
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			var result = new StringBuilder();
			var errorStr = _request.error;

			result.Append(_request.GetType().Name);
			result.Append(" (");
			result.Append(_request.url);

			if (IsFaulted)
			{
				if (string.IsNullOrEmpty(errorStr))
				{
					result.Append(", ");
					result.Append(_request.responseCode.ToString());
				}
				else
				{
					result.Append(", ");
					result.Append(errorStr);
					result.Append(" (");
					result.Append(_request.responseCode.ToString());
					result.Append(')');
				}
			}
			else if (IsCompleted)
			{
				result.Append(", ");
				result.Append(_request.responseCode.ToString());
			}

			result.Append(')');
			return result.ToString();
		}

		#endregion

		#region implementation

		private void SetCompleted()
		{
			try
			{
#if UNITY_5
				if (_request.isError)
#else
				if (_request.isHttpError || _request.isNetworkError)
#endif
				{
					throw new WebRequestException(_request.error, _request.responseCode);
				}
				else if (_request.downloadHandler != null)
				{
					var result = GetResult(_request);

					if (result != null)
					{
						TrySetResult(result);
					}
					else
					{
						throw new WebRequestException(string.Format("Failed to load {0} from URL: {1}.", typeof(T).Name, _request.url));
					}

					TrySetResult(result);
				}
				else
				{
					TrySetCompleted();
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		#endregion
	}
}
