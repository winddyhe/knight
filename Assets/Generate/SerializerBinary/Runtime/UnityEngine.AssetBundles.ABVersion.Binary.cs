using System.IO;
using Core;
using Core.Serializer;using Game.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace UnityEngine.AssetBundles
{
public partial class ABVersion
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.Entries);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.Entries = rReader.Deserialize(this.Entries);
	}
}
}
