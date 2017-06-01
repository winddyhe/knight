using System.IO;
using Core;
using Core.Serializer;using Game.Serializer;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace UnityEngine.AssetBundles
{
public partial class ABVersionEntry
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.N);
		rWriter.Serialize(this.V);
		rWriter.Serialize(this.M);
		rWriter.Serialize(this.S);
		rWriter.Serialize(this.D);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.N = rReader.Deserialize(this.N);
		this.V = rReader.Deserialize(this.V);
		this.M = rReader.Deserialize(this.M);
		this.S = rReader.Deserialize(this.S);
		this.D = rReader.Deserialize(this.D);
	}
}
}
