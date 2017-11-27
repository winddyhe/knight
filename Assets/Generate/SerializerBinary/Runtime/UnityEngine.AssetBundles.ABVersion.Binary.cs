
using System.IO;
using Core;
using Core.Serializer;
using Game.Serializer;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace UnityEngine.AssetBundles
{
	public partial class ABVersion
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.Entries);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.Entries = rReader.Deserialize(this.Entries);
		}
    }
}


