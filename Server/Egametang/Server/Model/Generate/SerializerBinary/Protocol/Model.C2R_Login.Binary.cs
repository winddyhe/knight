
using System.IO;
using Core;
using Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Model
{
	public partial class C2R_Login
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Account);
			rWriter.Serialize(this.Password);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Account = rReader.Deserialize(this.Account);
			this.Password = rReader.Deserialize(this.Password);
		}
    }
}


