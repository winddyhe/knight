using Knight.Framework.UI;
using System;

/// <summary>
/// Auto generate code, not need modify!
/// </summary>
namespace Game
{
	[DataBinding]
	public class SkillAssetConfigViewModel : ViewModel
	{
		[DataBinding]
		public String ID { get; set; }

		[DataBindingRelated("ID")]
		public String SkillEffectName => GameConfig.Instance.SkillAsset.Table[this.ID]?.SkillEffectName ?? default(String);

		[DataBindingRelated("ID")]
		public String SkillTimelineName => GameConfig.Instance.SkillAsset.Table[this.ID]?.SkillTimelineName ?? default(String);

		[DataBindingRelated("ID")]
		public String SkillScriptName => GameConfig.Instance.SkillAsset.Table[this.ID]?.SkillScriptName ?? default(String);

	}
}

