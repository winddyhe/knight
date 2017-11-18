using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Model
{
public partial class G2M_CreateUnit
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.PlayerId);
		rWriter.Serialize(this.GateSessionId);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.PlayerId = rReader.Deserialize(this.PlayerId);
		this.GateSessionId = rReader.Deserialize(this.GateSessionId);
	}
}
}
