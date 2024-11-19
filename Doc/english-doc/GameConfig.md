# GameConfig
* The framework processes game configuration files by converting Excel spreadsheets into a serialized binary file. This file is then packaged into an Assetbundle resource package for use in the game.
* The process begins with the ExcelExporter tool, which exports the Excel spreadsheet data into JSON files and C# GameConfig code files. Then, on the Unity side, a binary serialization method is used to generate a binary serialized file from the GameConfig code, and finally, the JSON configuration is converted into binary configuration data based on the serialized code.
* The configuration data classes in the game correspond to the hot update side.

## ExcelExporter
* The framework provides an external tool, ExcelExporter, which parses Excel files into JSON strings. The tool's path is knight/knight-tools/ExcelExporter/ExcelExporter.link.
* ![gameconfig_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_1.png)
* The Excel spreadsheets are located in the knight/knight-tools/ExcelExporter/Excels folder.
* The format of the Excel spreadsheet can be referenced in knight/knight-tools/ExcelExporter/Excels/Sample.xlsx
![gameconfig_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_2.png)
* First row: Configuration field names, used for generating C# code and JSON data.
* Second row: Configuration field type names, used for generating C# code.
* Third row: Field description.
* The first column is designated for the field name ID, which serves as the unique identifier for the table, and the types can be int32, int64, or string.

* The tool also supports a mode where a single table can have multiple sheets, and the exported file names are based on the sheet names.
* ![gameconfig_3](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_3.png)

## Exported Code and JSON Data
* The exported code is stored in the Assets/Game.Config/GameConfig/Generate folder.
* ![gameconfig_4](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_4.png)
* The generated code is shown as follows:
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
* Generate the corresponding serialization code for the generated configuration code.
* ![gameconfig_5](https://github.com/winddyhe/knight/blob/master/Doc/res/images/gameconfig_5.png)
* The generated serialization code can be found in Assets\Game.Config\Generate\Serialize\GameConfig, as shown below.
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
* The exported JSON configuration file is located at: Assets\GameAssets\Config\GameConfig\Json. After exporting the JSON file, the framework will automatically generate a bytes file based on changes to the JSON configuration file, located at Assets\GameAssets\Config\GameConfig\Binary, for use in packaging the asset bundle.
* Before resource packaging, the packaging tool will first re-serialize the configured JSON data into a binary data file, and then process the binary file for packaging, to prevent inaccuracies in converting JSON to bytes.