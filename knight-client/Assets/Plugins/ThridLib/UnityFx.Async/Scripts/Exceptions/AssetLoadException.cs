// Copyright (c) Alexander Bogarsukov.
// Licensed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using System.Runtime.Serialization;

namespace UnityFx.Async
{
	/// <summary>
	/// Represents an asset loading error.
	/// </summary>
	public class AssetLoadException : Exception
	{
		#region data

		private const string _assetNameSerializationName = "_assetName";
		private const string _assetTypeSerializationName = "_assetType";

		private readonly Type _assetType;
		private readonly string _assetName;

		#endregion

		#region interface

		/// <summary>
		/// Gets an asset name.
		/// </summary>
		public string AssetName
		{
			get
			{
				return _assetName;
			}
		}

		/// <summary>
		/// Gets an asset type.
		/// </summary>
		public Type AssetType
		{
			get
			{
				return _assetType;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException()
			: base("Failed to load an asset.")
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string message, string assetName, Type assetType)
			: base(message)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string message, string assetName, Type assetType, Exception innerException)
			: base(message, innerException)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string assetName, Type assetType)
			: base(string.Format("Failed to load {0} with name {1}.", assetType, assetName))
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(string assetName, Type assetType, Exception innerException)
			: base(string.Format("Failed to load {0} with name {1}.", assetType, assetName), innerException)
		{
			_assetName = assetName;
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(Type assetType)
			: base(string.Format("Failed to load {0}.", assetType))
		{
			_assetType = assetType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssetLoadException"/> class.
		/// </summary>
		public AssetLoadException(Type assetType, Exception innerException)
			: base(string.Format("Failed to load {0}.", assetType), innerException)
		{
			_assetType = assetType;
		}

		#endregion

		#region ISerializable

		private AssetLoadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_assetName = info.GetString(_assetNameSerializationName);
			_assetType = (Type)info.GetValue(_assetTypeSerializationName, typeof(Type));
		}

		/// <inheritdoc/>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue(_assetNameSerializationName, _assetName);
			info.AddValue(_assetTypeSerializationName, _assetType);
		}

		#endregion
	}
}
