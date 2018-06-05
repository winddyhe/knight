
using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class ActorProfessional
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.ID);
			rWriter.Serialize(this.HeroID);
			rWriter.Serialize(this.Name);
			rWriter.Serialize(this.Desc);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.ID = rReader.Deserialize(this.ID);
			this.HeroID = rReader.Deserialize(this.HeroID);
			this.Name = rReader.Deserialize(this.Name);
			this.Desc = rReader.Deserialize(this.Desc);
		}
    }
}


