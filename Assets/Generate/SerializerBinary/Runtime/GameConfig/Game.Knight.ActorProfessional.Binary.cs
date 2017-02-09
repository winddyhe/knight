using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Game.Knight
{
public partial class ActorProfessional
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.ID);
		rWriter.Serialize(this.HeroID);
		rWriter.Serialize(this.Name);
		rWriter.Serialize(this.Desc);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.ID = rReader.Deserialize(this.ID);
		this.HeroID = rReader.Deserialize(this.HeroID);
		this.Name = rReader.Deserialize(this.Name);
		this.Desc = rReader.Deserialize(this.Desc);
	}
}
}
