using Knight.Framework.UI;
using System;

/// <summary>
/// Auto generate code, not need modify!
/// </summary>
namespace Game
{
	[DataBinding]
	public class LanguageConfigViewModel : ViewModel
	{
		[DataBinding]
		public String ID { get; set; }

		[DataBindingRelated("ID")]
		public String ChineseSimplified => GameConfig.Instance.Language.Table[this.ID]?.ChineseSimplified ?? default(String);

		[DataBindingRelated("ID")]
		public String ChineseTraditional => GameConfig.Instance.Language.Table[this.ID]?.ChineseTraditional ?? default(String);

		[DataBindingRelated("ID")]
		public String English => GameConfig.Instance.Language.Table[this.ID]?.English ?? default(String);

		[DataBindingRelated("ID")]
		public String Japanese => GameConfig.Instance.Language.Table[this.ID]?.Japanese ?? default(String);

		[DataBindingRelated("ID")]
		public String Indonesian => GameConfig.Instance.Language.Table[this.ID]?.Indonesian ?? default(String);

		[DataBindingRelated("ID")]
		public String Korean => GameConfig.Instance.Language.Table[this.ID]?.Korean ?? default(String);

		[DataBindingRelated("ID")]
		public String Malay => GameConfig.Instance.Language.Table[this.ID]?.Malay ?? default(String);

		[DataBindingRelated("ID")]
		public String Thai => GameConfig.Instance.Language.Table[this.ID]?.Thai ?? default(String);

		[DataBindingRelated("ID")]
		public String Arabic => GameConfig.Instance.Language.Table[this.ID]?.Arabic ?? default(String);

	}
}

