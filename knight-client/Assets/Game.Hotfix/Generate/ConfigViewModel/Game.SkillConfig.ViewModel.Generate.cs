using Knight.Framework.UI;
using System;

/// <summary>
/// Auto generate code, not need modify!
/// </summary>
namespace Game
{
	[DataBinding]
	public class SkillConfigViewModel : ViewModel
	{
		[DataBinding]
		public Int32 ID { get; set; }

		[DataBindingRelated("ID")]
		public String SkillName => GameConfig.Instance.Skill.Table[this.ID]?.SkillName ?? default(String);

		[DataBindingRelated("ID")]
		public Int32 SkillCastTargetType => GameConfig.Instance.Skill.Table[this.ID]?.SkillCastTargetType ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 TargetSelectCampType => GameConfig.Instance.Skill.Table[this.ID]?.TargetSelectCampType ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 TargetSelectSearchType => GameConfig.Instance.Skill.Table[this.ID]?.TargetSelectSearchType ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 TargetSelectRadiusOrHeight => GameConfig.Instance.Skill.Table[this.ID]?.TargetSelectRadiusOrHeight ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 TargetSelectAngleOrWidth => GameConfig.Instance.Skill.Table[this.ID]?.TargetSelectAngleOrWidth ?? default(Int32);

	}
}

