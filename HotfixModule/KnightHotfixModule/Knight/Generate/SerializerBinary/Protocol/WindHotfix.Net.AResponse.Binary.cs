
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace WindHotfix.Net
{
	public partial class AResponse
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.RpcId);
			rWriter.Serialize(this.Error);
			rWriter.Serialize(this.Message);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.RpcId = rReader.Deserialize(this.RpcId);
			this.Error = rReader.Deserialize(this.Error);
			this.Message = rReader.Deserialize(this.Message);
		}
    }
}


