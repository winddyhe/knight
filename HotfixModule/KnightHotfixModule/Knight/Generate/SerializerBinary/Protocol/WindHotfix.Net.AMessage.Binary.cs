
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace WindHotfix.Net
{
	public partial class AMessage
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
		}
    }
}


