using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Model
{
public partial class AResponse
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.RpcId);
		rWriter.Serialize(this.Error);
		rWriter.Serialize(this.Message);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.RpcId = rReader.Deserialize(this.RpcId);
		this.Error = rReader.Deserialize(this.Error);
		this.Message = rReader.Deserialize(this.Message);
	}
}
}
