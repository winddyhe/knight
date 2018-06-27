using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace ETHotfix
{
	public partial class PlayerInfo
	{
		public override void Serialize(BinaryWriter rWriter)
		{
			base.Serialize(rWriter);
		}
		public override void Deserialize(BinaryReader rReader)
		{
			base.Deserialize(rReader);
		}
	}
}

