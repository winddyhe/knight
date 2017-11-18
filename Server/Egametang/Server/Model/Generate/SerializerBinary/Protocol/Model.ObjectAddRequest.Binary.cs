using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Model
{
public partial class ObjectAddRequest
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.Key);
		rWriter.Serialize(this.AppId);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.Key = rReader.Deserialize(this.Key);
		this.AppId = rReader.Deserialize(this.AppId);
	}
}
}
