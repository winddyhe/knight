using System.IO;
using Core;
using WindHotfix.Core;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace KnightHotfixModule.Knight
{
public partial class GameConfig
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.Avatars);
		rWriter.Serialize(this.Heros);
		rWriter.Serialize(this.ActorProfessionals);
		rWriter.Serialize(this.StageConfigs);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.Avatars = rReader.Deserialize(this.Avatars);
		this.Heros = rReader.Deserialize(this.Heros);
		this.ActorProfessionals = rReader.Deserialize(this.ActorProfessionals);
		this.StageConfigs = rReader.Deserialize(this.StageConfigs);
	}
}
}
