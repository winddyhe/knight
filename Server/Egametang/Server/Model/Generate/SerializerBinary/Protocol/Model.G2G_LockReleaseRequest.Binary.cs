using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Model
{
public partial class G2G_LockReleaseRequest
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.Id);
		rWriter.Serialize(this.Address);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.Id = rReader.Deserialize(this.Id);
		this.Address = rReader.Deserialize(this.Address);
	}
}
}
