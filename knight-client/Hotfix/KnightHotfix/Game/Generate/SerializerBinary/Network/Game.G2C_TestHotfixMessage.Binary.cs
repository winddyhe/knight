
using System.IO;
using Knight.Hotfix.Core;
using Knight.Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game
{
	public partial class G2C_TestHotfixMessage
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Info);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Info = rReader.Deserialize(this.Info);
		}
    }
}


