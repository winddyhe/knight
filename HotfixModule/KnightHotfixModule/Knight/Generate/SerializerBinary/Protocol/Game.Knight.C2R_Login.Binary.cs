using System.IO;
using Core;
using WindHotfix.Core;


/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Game.Knight
{
public partial class C2R_Login
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.Account);
		rWriter.Serialize(this.Password);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.Account = rReader.Deserialize(this.Account);
		this.Password = rReader.Deserialize(this.Password);
	}
}
}
