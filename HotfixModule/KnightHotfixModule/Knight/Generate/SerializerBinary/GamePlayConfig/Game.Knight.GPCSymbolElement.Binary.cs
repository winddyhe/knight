
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class GPCSymbolElement
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Identifer);
			rWriter.Serialize(this.Args);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Identifer = rReader.Deserialize(this.Identifer);
			this.Args = rReader.Deserialize(this.Args);
		}
    }
}


