
using System.IO;
using Core;
using Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Model
{
	public partial class Actor_TransferRequest
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.MapIndex);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.MapIndex = rReader.Deserialize(this.MapIndex);
		}
    }
}


