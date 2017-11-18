using System.IO;
using Core;
using Core.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Model
{
public partial class DBSaveBatchRequest
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.NeedCache);
		rWriter.Serialize(this.CollectionName);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.NeedCache = rReader.Deserialize(this.NeedCache);
		this.CollectionName = rReader.Deserialize(this.CollectionName);
	}
}
}
