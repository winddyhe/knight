
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class UnitInfo
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.UnitId);
			rWriter.Serialize(this.X);
			rWriter.Serialize(this.Z);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.UnitId = rReader.Deserialize(this.UnitId);
			this.X = rReader.Deserialize(this.X);
			this.Z = rReader.Deserialize(this.Z);
		}
    }
}


