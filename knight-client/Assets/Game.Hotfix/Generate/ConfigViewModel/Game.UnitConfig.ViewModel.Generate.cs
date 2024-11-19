using Knight.Framework.UI;
using System;

/// <summary>
/// Auto generate code, not need modify!
/// </summary>
namespace Game
{
	[DataBinding]
	public class UnitConfigViewModel : ViewModel
	{
		[DataBinding]
		public Int32 ID { get; set; }

		[DataBindingRelated("ID")]
		public Int32 ActorType => GameConfig.Instance.Unit.Table[this.ID]?.ActorType ?? default(Int32);

		[DataBindingRelated("ID")]
		public String Name => GameConfig.Instance.Unit.Table[this.ID]?.Name ?? default(String);

		[DataBindingRelated("ID")]
		public String PrefabABPath => GameConfig.Instance.Unit.Table[this.ID]?.PrefabABPath ?? default(String);

		[DataBindingRelated("ID")]
		public Int32 ModelScale => GameConfig.Instance.Unit.Table[this.ID]?.ModelScale ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 NormalSkillIntervalTime => GameConfig.Instance.Unit.Table[this.ID]?.NormalSkillIntervalTime ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32[] NormalSkills => GameConfig.Instance.Unit.Table[this.ID]?.NormalSkills ?? default(Int32[]);

		[DataBindingRelated("ID")]
		public Int32 Skill1 => GameConfig.Instance.Unit.Table[this.ID]?.Skill1 ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 Skill2 => GameConfig.Instance.Unit.Table[this.ID]?.Skill2 ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 Skill3 => GameConfig.Instance.Unit.Table[this.ID]?.Skill3 ?? default(Int32);

		[DataBindingRelated("ID")]
		public Int32 Skill4 => GameConfig.Instance.Unit.Table[this.ID]?.Skill4 ?? default(Int32);

	}
}

