using System.IO;
using Core;
using WindHotfix.Core;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace KnightHotfixModule.Knight
{
public partial class Avatar
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.ID);
		rWriter.Serialize(this.AvatarName);
		rWriter.Serialize(this.ABPath);
		rWriter.Serialize(this.AssetName);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.ID = rReader.Deserialize(this.ID);
		this.AvatarName = rReader.Deserialize(this.AvatarName);
		this.ABPath = rReader.Deserialize(this.ABPath);
		this.AssetName = rReader.Deserialize(this.AssetName);
	}
}
}
