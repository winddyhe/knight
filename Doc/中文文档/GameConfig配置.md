# GameConfig配置
* 框架中的对游戏配置文件的处理是将Excel表格最终处理为一个序列化之后的二进制文件。然后再打包成为一个Assetbundle资源包，供游戏使用。
* 其过程是先通过ExcelExporter导出工具将Excel表格数据导出为Json文件和C#的GameConfig代码文件。然后在Unity端通过二进制序列化生成的方式，将GameConfig代码生成一份二进制序列化文件，最后根据序列化代码将json配置转成二进制配置数据。
* 游戏中的配置对应的数据类在热更新端。

## ExcelExporter
* 框架中提供了一个外部工具，将Excel解析为Json字符串的工具ExcelExporter，工具路径是knight/knight-tools/ExcelExporter/ExcelExporter.link。
* ![gameconfig_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_1.png)
* Excel表格在knight/knight-tools/ExcelExporter/Excels文件夹中。
* Excel表格格式参考knight/knight-tools/ExcelExporter/Excels/Sample.xlsx
	![gameconfig_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_2.png)
	* 第一行: 配置字段名字，用于生成C#代码和json数据使用
	* 第二行: 配置字段的类型名字，用于生成C#代码使用
	* 第三行: 字段描述说明
	* 第一列约定字段名字为ID，最为表格唯一标识符使用，类型可以是int32、int64、string
	
* 工具还支持一个表格多个Sheet的模式，导出的文件名是以Sheet名字为准。
* ![gameconfig_3](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_3.png)

## 导出的代码和Json数据
* 导出的代码存放的位置在Assets/Game.Config/GameConfig/Generate文件夹中。
* ![gameconfig_4](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_4.png)
* 生成的代码如下所示：
```C#
    [SerializerBinary]
    [SBGroup("GameConfig")]
    /// <summary>
    /// Auto generate code, don't modify it.
    /// </summary>
    public partial class SkillConfig
    {
        public int ID;
        public string SkillName;
        public int SkillCastTargetType;
        public int TargetSelectCampType;
        public int TargetSelectSearchType;
        public int TargetSelectRadiusOrHeight;
        public int TargetSelectAngleOrWidth;
    }
```
* 将生成的配置代码对应生成他的序列化代码。
* ![gameconfig_5](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_5.png)
* 生成的序列化代码在Assets\Game.Config\Generate\Serialize\GameConfig中，如下所示。
```C#
	public partial class SkillConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.SkillCastTargetType);
			rWriter.Serialize(this.SkillName);
			rWriter.Serialize(this.TargetSelectAngleOrWidth);
			rWriter.Serialize(this.TargetSelectCampType);
			rWriter.Serialize(this.TargetSelectRadiusOrHeight);
			rWriter.Serialize(this.TargetSelectSearchType);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ID = rReader.Deserialize(this.ID);
			this.SkillCastTargetType = rReader.Deserialize(this.SkillCastTargetType);
			this.SkillName = rReader.Deserialize(this.SkillName);
			this.TargetSelectAngleOrWidth = rReader.Deserialize(this.TargetSelectAngleOrWidth);
			this.TargetSelectCampType = rReader.Deserialize(this.TargetSelectCampType);
			this.TargetSelectRadiusOrHeight = rReader.Deserialize(this.TargetSelectRadiusOrHeight);
			this.TargetSelectSearchType = rReader.Deserialize(this.TargetSelectSearchType);
		}
	}
```
* 导出的Json配置文件在路径：Assets\GameAssets\Config\GameConfig\Json下，在导出json文件之后，框架层会根据json配置文件的变化自动生成一份bytes文件，路径在Assets\GameAssets\Config\GameConfig\Binary下，供打包assetbundle使用。
* 在资源打包之前，打包工具会先根据配置的json数据重新序列化为二进制数据文件，再对二进制文件进行打包处理，以防止json转bytes的不准确。

