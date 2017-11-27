
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class C2G_LoginGate
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Key);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Key = rReader.Deserialize(this.Key);
		}
    }
}


