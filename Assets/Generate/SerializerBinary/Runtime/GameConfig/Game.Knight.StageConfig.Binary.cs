using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Game.Knight
{
public partial class StageConfig
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.StageID);
		rWriter.Serialize(this.SceneABPath);
		rWriter.Serialize(this.ScenePath);
		rWriter.Serialize(this.BornPos);
		rWriter.Serialize(this.CameraSettings);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.StageID = rReader.Deserialize(this.StageID);
		this.SceneABPath = rReader.Deserialize(this.SceneABPath);
		this.ScenePath = rReader.Deserialize(this.ScenePath);
		this.BornPos = rReader.Deserialize(this.BornPos);
		this.CameraSettings = rReader.Deserialize(this.CameraSettings);
	}
}
}
