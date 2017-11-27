
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class GPCSymbolObject
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Head);
			rWriter.Serialize(this.Bodies);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Head = rReader.Deserialize(this.Head);
			this.Bodies = rReader.Deserialize(this.Bodies);
		}
    }
}


