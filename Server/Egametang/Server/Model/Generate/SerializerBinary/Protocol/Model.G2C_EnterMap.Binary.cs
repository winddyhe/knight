
using System.IO;
using Core;
using Core.Serializer;

/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Model
{
	public partial class G2C_EnterMap
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.UnitId);
			rWriter.Serialize(this.Count);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.UnitId = rReader.Deserialize(this.UnitId);
			this.Count = rReader.Deserialize(this.Count);
		}
    }
}


