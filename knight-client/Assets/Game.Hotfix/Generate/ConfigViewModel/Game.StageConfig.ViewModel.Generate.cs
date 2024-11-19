using Knight.Framework.UI;
using System;

/// <summary>
/// Auto generate code, not need modify!
/// </summary>
namespace Game
{
	[DataBinding]
	public class StageConfigViewModel : ViewModel
	{
		[DataBinding]
		public Int32 ID { get; set; }

		[DataBindingRelated("ID")]
		public String SceneName => GameConfig.Instance.Stage.Table[this.ID]?.SceneName ?? default(String);

		[DataBindingRelated("ID")]
		public Int32[] Heros => GameConfig.Instance.Stage.Table[this.ID]?.Heros ?? default(Int32[]);

	}
}

