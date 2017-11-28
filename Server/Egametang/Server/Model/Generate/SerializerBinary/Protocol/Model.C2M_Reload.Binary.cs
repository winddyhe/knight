
using System.IO;
using Core;
using Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Model
{
	public partial class C2M_Reload
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize((int)this.AppType);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.AppType = (Model.AppType)rReader.Deserialize((int)this.AppType);
		}
    }
}


