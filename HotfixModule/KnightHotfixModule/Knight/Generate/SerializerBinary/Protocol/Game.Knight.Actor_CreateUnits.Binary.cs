
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class Actor_CreateUnits
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Units);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Units = rReader.Deserialize(this.Units);
		}
    }
}


