
using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class GameConfig
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Avatars);
			rWriter.Serialize(this.Heros);
			rWriter.Serialize(this.ActorProfessionals);
			rWriter.Serialize(this.StageConfigs);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Avatars = rReader.Deserialize(this.Avatars);
			this.Heros = rReader.Deserialize(this.Heros);
			this.ActorProfessionals = rReader.Deserialize(this.ActorProfessionals);
			this.StageConfigs = rReader.Deserialize(this.StageConfigs);
		}
    }
}


