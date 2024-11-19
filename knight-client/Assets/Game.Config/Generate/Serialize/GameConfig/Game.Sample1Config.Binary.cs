using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:02BFB9A2E069D8220A8B700D5C8CA923

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class Sample1Config : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.Test1);
			CommonSerializer.Serialize(rWriter, this.Test10);
			CommonSerializer.Serialize(rWriter, this.Test11);
			CommonSerializer.Serialize(rWriter, this.Test12);
			CommonSerializer.Serialize(rWriter, this.Test13);
			CommonSerializer.Serialize(rWriter, this.Test14);
			CommonSerializer.Serialize(rWriter, this.Test15);
			rWriter.Serialize(this.Test2);
			rWriter.Serialize(this.Test3);
			rWriter.Serialize(this.Test4);
			rWriter.Serialize(this.Test5_Lan);
			CommonSerializer.Serialize(rWriter, this.Test6);
			CommonSerializer.Serialize(rWriter, this.Test7);
			CommonSerializer.Serialize(rWriter, this.Test8);
			CommonSerializer.Serialize(rWriter, this.Test9);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ID = rReader.Deserialize(this.ID);
			this.Test1 = rReader.Deserialize(this.Test1);
			this.Test10 = CommonSerializer.Deserialize(rReader, this.Test10);
			this.Test11 = CommonSerializer.Deserialize(rReader, this.Test11);
			this.Test12 = CommonSerializer.Deserialize(rReader, this.Test12);
			this.Test13 = CommonSerializer.Deserialize(rReader, this.Test13);
			this.Test14 = CommonSerializer.Deserialize(rReader, this.Test14);
			this.Test15 = CommonSerializer.Deserialize(rReader, this.Test15);
			this.Test2 = rReader.Deserialize(this.Test2);
			this.Test3 = rReader.Deserialize(this.Test3);
			this.Test4 = rReader.Deserialize(this.Test4);
			this.Test5_Lan = rReader.Deserialize(this.Test5_Lan);
			this.Test6 = CommonSerializer.Deserialize(rReader, this.Test6);
			this.Test7 = CommonSerializer.Deserialize(rReader, this.Test7);
			this.Test8 = CommonSerializer.Deserialize(rReader, this.Test8);
			this.Test9 = CommonSerializer.Deserialize(rReader, this.Test9);
		}
	}
}

/// <summary>
/// CommonSerializer private ref
/// </summary>
namespace Game
{
	public static partial class CommonSerializer
	{
		public static Game.Sample1Config Deserialize(BinaryReader rReader, Game.Sample1Config value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.Sample1Config();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
