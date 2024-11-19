using System.IO;
using Knight.Core;
using Knight.Framework.Serializer;
using Game;

//ScriptMD5:853F3136030DEAAF3AD1E70C807C30C8

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class UnitConfig : ISerializerBinary
	{
		public void Serialize(BinaryWriter rWriter)
		{
			rWriter.Serialize(this.ActorType);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.ModelScale);
			rWriter.Serialize(this.Name);
			rWriter.Serialize(this.NormalSkillIntervalTime);
			CommonSerializer.Serialize(rWriter, this.NormalSkills);
			rWriter.Serialize(this.PrefabABPath);
			rWriter.Serialize(this.Skill1);
			rWriter.Serialize(this.Skill2);
			rWriter.Serialize(this.Skill3);
			rWriter.Serialize(this.Skill4);
		}
		public void Deserialize(BinaryReader rReader)
		{
			this.ActorType = rReader.Deserialize(this.ActorType);
			this.ID = rReader.Deserialize(this.ID);
			this.ModelScale = rReader.Deserialize(this.ModelScale);
			this.Name = rReader.Deserialize(this.Name);
			this.NormalSkillIntervalTime = rReader.Deserialize(this.NormalSkillIntervalTime);
			this.NormalSkills = CommonSerializer.Deserialize(rReader, this.NormalSkills);
			this.PrefabABPath = rReader.Deserialize(this.PrefabABPath);
			this.Skill1 = rReader.Deserialize(this.Skill1);
			this.Skill2 = rReader.Deserialize(this.Skill2);
			this.Skill3 = rReader.Deserialize(this.Skill3);
			this.Skill4 = rReader.Deserialize(this.Skill4);
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
		public static Game.UnitConfig Deserialize(BinaryReader rReader, Game.UnitConfig value)
		{
			var bValid = rReader.Deserialize(default(bool));
			if (!bValid) return null;

			var rResult = new Game.UnitConfig();
			rResult.Deserialize(rReader);
			return rResult;
		}

	}
}
