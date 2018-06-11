
using System.IO;
using Knight.Core;
using Knight.Core.Serializer;
using Knight.Framework.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Knight.Framework.Net
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


