// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace UnityFx.Async
{
	/// <summary>
	/// Represents a web request error.
	/// </summary>
	public class WebRequestException : Exception
	{
		#region data

		private const string _responseCodeSerializationName = "_reason";

		private readonly long _responseCode;

		#endregion

		#region interface

		/// <summary>
		/// Gets a response code for the source web request.
		/// </summary>
		public long ResponseCode
		{
			get
			{
				return _responseCode;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestException"/> class.
		/// </summary>
		public WebRequestException()
			: base("UnityWebRequest error.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestException"/> class.
		/// </summary>
		public WebRequestException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestException"/> class.
		/// </summary>
		public WebRequestException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestException"/> class.
		/// </summary>
		public WebRequestException(string message, long responseCode)
			: base(message)
		{
			_responseCode = responseCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebRequestException"/> class.
		/// </summary>
		public WebRequestException(string message, long responseCode, Exception innerException)
			: base(message, innerException)
		{
			_responseCode = responseCode;
		}

		#endregion

		#region ISerializable

		private WebRequestException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_responseCode = info.GetInt64(_responseCodeSerializationName);
		}

		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue(_responseCodeSerializationName, _responseCode);
		}

		#endregion
	}
}
