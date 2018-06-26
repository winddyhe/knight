using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.AssetBundles
{
	public partial class ABVersion
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
			rWriter.Serialize(this.Entries);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
			this.Entries = rReader.Deserialize(this.Entries);
		}
	}
}

