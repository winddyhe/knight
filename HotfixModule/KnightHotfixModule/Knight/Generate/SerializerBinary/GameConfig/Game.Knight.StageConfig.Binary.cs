
using System.IO;
using WindHotfix.Core;
using Core;


/// <summary>
/// Auto generate code, not need modify.
/// </summary>
namespace Game.Knight
{
	public partial class StageConfig
	{
        public override void Serialize(BinaryWriter rWriter)
	    {
            base.Serialize(rWriter);
			rWriter.Serialize(this.StageID);
			rWriter.Serialize(this.SceneABPath);
			rWriter.Serialize(this.ScenePath);
			rWriter.Serialize(this.BornPos);
			rWriter.Serialize(this.CameraSettings);
		}
		public override void Deserialize(BinaryReader rReader)
	    {
		    base.Deserialize(rReader);
			this.StageID = rReader.Deserialize(this.StageID);
			this.SceneABPath = rReader.Deserialize(this.SceneABPath);
			this.ScenePath = rReader.Deserialize(this.ScenePath);
			this.BornPos = rReader.Deserialize(this.BornPos);
			this.CameraSettings = rReader.Deserialize(this.CameraSettings);
		}
    }
}


