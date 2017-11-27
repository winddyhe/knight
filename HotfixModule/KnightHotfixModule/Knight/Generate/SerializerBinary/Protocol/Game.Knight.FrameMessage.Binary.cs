
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class FrameMessage
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Frame);
			rWriter.Serialize(this.Messages);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Frame = rReader.Deserialize(this.Frame);
			this.Messages = rReader.Deserialize(this.Messages);
		}
    }
}


