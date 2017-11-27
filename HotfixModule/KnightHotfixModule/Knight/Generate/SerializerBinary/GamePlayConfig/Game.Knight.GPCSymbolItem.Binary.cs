
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class GPCSymbolItem
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Value);
			rWriter.Serialize((int)this.Type);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Value = rReader.Deserialize(this.Value);
			this.Type = (Game.Knight.GPCSymbolType)rReader.Deserialize((int)this.Type);
		}
    }
}


