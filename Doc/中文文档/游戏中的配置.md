# 游戏中的配置
* 框架中的对游戏配置文件的处理是将Excel表格最终处理为一个序列化之后的二进制文件。然后再打包成为一个Assetbundle资源包，供游戏使用。
* 游戏中的配置对应的数据类在热更新端。

## ExcelReader
* 框架中提供了一个将Excel解析为Json字符串的工具，ExcelReader，菜单路径是Tools/AssetBudle/Export Excel Config。
* ![gameconfig_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_1.png)
* 目前Excel表格的路径是固定的：只能在Assets/Game/Knight/GameAsset/Config/Excel目录下，当然你也可以修改导出工具的代码来修改路径。
* 在Assets/Editor/ExcelReader文件夹下有一个表格数据格式的配置文件excel_format_config.json，需要对导出数据进行配置。
* ![gameconfig_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_2.png)
	* ExcelName: Excel表的文件名
	* SheetName: 表的Sheet名字
	* ClassName: 表数据对应代码中的类名
	* PrimaryKey: 配置将会统一导出成为Dictionary格式的json字符串，因此需要在表中找一列来做Dict的Key值
	
* 工具还支持针对单个表格的导出。
* ![gameconfig_3](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_3.png)

## Json数据的序列化
* 在热更新DLL中有一个GameConfig类，里面用来存放所有的游戏配置数据。
* ![gameconfig_4](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_4.png)

* 单个数据的序列化代码是通过一个序列化工具来自动生成的，配置只需要继承HotfixSerializerBinary类即可。HotfixSBGroup标签是用来对配置进行分组的。
```C#
    [HotfixSBGroup("GameConfig")]
    public partial class ActorHero : HotfixSerializerBinary
    {
        public int      ID;
        public string   Name;
        public int      AvatarID;
        public int      SkillID;
        public float    Scale;
        public float    Height;
        public float    Radius;
    }
```
* 在热更工程中，提供了一个SerializerBinaryEditor工具，用来自动生成配置类的序列化和反序列化代码，省去了自己写序列化代码。
* ![gameconfig_5](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_5.png)
* 生成的序列化代码在knight\HotfixModule\KnightHotfixModule\Knight\Generate\SerializerBinary中，如下所示。
```C#
using System.IO;
using Core;
using WindHotfix.Core;

/// <summary>
/// 文件自动生成无需又该！如果出现编译错误，删除文件后会自动生成
/// </summary>
namespace Game.Knight
{
public partial class ActorHero
{
	public override void Serialize(BinaryWriter rWriter)
	{
		base.Serialize(rWriter);
		rWriter.Serialize(this.ID);
		rWriter.Serialize(this.Name);
		rWriter.Serialize(this.AvatarID);
		rWriter.Serialize(this.SkillID);
		rWriter.Serialize(this.Scale);
		rWriter.Serialize(this.Height);
		rWriter.Serialize(this.Radius);
	}
	public override void Deserialize(BinaryReader rReader)
	{
		base.Deserialize(rReader);
		this.ID = rReader.Deserialize(this.ID);
		this.Name = rReader.Deserialize(this.Name);
		this.AvatarID = rReader.Deserialize(this.AvatarID);
		this.SkillID = rReader.Deserialize(this.SkillID);
		this.Scale = rReader.Deserialize(this.Scale);
		this.Height = rReader.Deserialize(this.Height);
		this.Radius = rReader.Deserialize(this.Radius);
	}
}
}
```

* 在资源打包之前，打包工具会先根据配置的json数据重新序列化为二进制数据文件。文件路径是Assets\Game\Knight\GameAsset\Config\GameConfig\Binary\GameConfig.bytes。再对二进制文件进行打包处理。

## TODO
* 以上这种方式，适合于配置比较少的游戏。对于配置数据较多的游戏这样的配置处理方法很占用Mono内存。在后期将会加入sqlite存储配置数据的解决方案。

