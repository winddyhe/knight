
using System.IO;
using Core;
using Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Model
{
	public partial class R2C_Login
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Address);
			rWriter.Serialize(this.Key);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Address = rReader.Deserialize(this.Address);
			this.Key = rReader.Deserialize(this.Key);
		}
    }
}


