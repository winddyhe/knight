// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Text;
using UnityEngine;

namespace UnityFx.Async.Helpers
{
#if !UNITY_2018_3_OR_NEWER

	/// <summary>
	/// A wrapper for <see cref="WWW"/> with result value.
	/// </summary>
	/// <typeparam name="T">Type of the request result.</typeparam>
	internal class WwwResult<T> : AsyncResult<T> where T : class
	{
		#region data

		private readonly WWW _www;

		#endregion

		#region interface

		/// <summary>
		/// Gets the underlying <see cref="WWW"/> instance.
		/// </summary>
		public WWW WebRequest
		{
			get
			{
				return _www;
			}
		}

		/// <summary>
		/// Gets the request url string.
		/// </summary>
		public string Url
		{
			get
			{
				return _www.url;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WwwResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		public WwwResult(WWW request)
		{
			_www = request;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WwwResult{T}"/> class.
		/// </summary>
		/// <param name="request">Source web request.</param>
		/// <param name="userState">User-defined data.</param>
		public WwwResult(WWW request, object userState)
			: base(null, userState)
		{
			_www = request;
		}

		/// <summary>
		/// Initializes the operation result value. Called when the underlying <see cref="WWW"/> has completed withou errors.
		/// </summary>
		protected virtual T GetResult(WWW request)
		{
			return AsyncWww.GetResult<T>(request);
		}

		#endregion

		#region AsyncResult

		/// <inheritdoc/>
		protected override float GetProgress()
		{
			return _www.progress;
		}

		/// <inheritdoc/>
		protected override void OnStarted()
		{
			if (_www.isDone)
			{
				SetCompleted();
			}
			else
			{
				AsyncUtility.AddCompletionCallback(_www, SetCompleted);
			}
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_www.Dispose();
			}

			base.Dispose(disposing);
		}

		#endregion

		#region Object

		/// <inheritdoc/>
		public override string ToString()
		{
			var result = new StringBuilder();
			var errorStr = _www.error;

			result.Append(_www.GetType().Name);
			result.Append(" (");
			result.Append(_www.url);

			if (IsFaulted && !string.IsNullOrEmpty(errorStr))
			{
				result.Append(", ");
				result.Append(errorStr);
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
				if (string.IsNullOrEmpty(_www.error))
				{
					var result = GetResult(_www);

					if (result != null)
					{
						TrySetResult(result);
					}
					else
					{
						throw new WebRequestException(string.Format("Failed to load {0} from URL: {1}.", typeof(T).Name, _www.url));
					}
				}
				else
				{
					TrySetException(new WebRequestException(_www.error));
				}
			}
			catch (Exception e)
			{
				TrySetException(e);
			}
		}

		#endregion
	}

#endif
}
