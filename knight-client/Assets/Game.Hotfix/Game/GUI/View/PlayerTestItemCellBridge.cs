using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Knight.Framework.UI;

///<summary>
/// Auto Generate By Knight, Not Edit It Manually.
///</summary>
namespace Game
{
	public class PlayerTestItemCellBridge : FancyScrollCellBridge
	{
		public int Index;
		public ViewModel ViewModel;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__Sheet_Name__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem> __TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0;
		public TMPro.TextMeshProUGUI __TextMeshProUGUI__Sheet_Password__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem> __TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0;
		public TMPro.TextMeshProUGUI __TextMeshProUGUI__Sheet_BtnComfirm_Text__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem> __TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0;
		public TMPro.TMP_InputField __TMP_InputField__Sheet_InputField__;
		public BindableViewModel<TMPro.TMP_InputField, Game.PlayerTestItem> __TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0;
		public UnityEngine.UI.Button __Button__Sheet_BtnComfirm__;

		public override void Initialize()
		{
			this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem>();
			this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem>();
			this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerTestItem>();
			this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0 = new BindableViewModel<TMPro.TMP_InputField, Game.PlayerTestItem>();
		}

		public override void UpdateContent(int nIndex, FancyScrollCellData rCellData)
		{
			if (this.ViewModel != rCellData.ViewModel)
			{
				if (this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel != null)
					this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel.UnregisterPropertyChangeHandler<String>
						(nameof(Game.PlayerTestItem.Test1), this.Set__TextMeshProUGUI__Sheet_Name___text);
				if (this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel != null)
					this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel.UnregisterPropertyChangeHandler<String>
						(nameof(Game.PlayerTestItem.Test2), this.Set__TextMeshProUGUI__Sheet_Password___text);
				if (this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0.ViewModel != null)
					this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0.ViewModel.UnregisterPropertyChangeHandler<System.String>
						(nameof(Game.PlayerTestItem.Test2), this.Set__TextMeshProUGUI__Sheet_BtnComfirm_Text___text_Related_Test2);
				if (this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel != null)
					this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel.UnregisterPropertyChangeHandler<String>
						(nameof(Game.PlayerTestItem.Test2), this.Set__TMP_InputField__Sheet_InputField___text);

				var rNewItem = rCellData.ViewModel as Game.PlayerTestItem;
				this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel = rNewItem;
				this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel = rNewItem;
				this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0.ViewModel = rNewItem;
				this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel = rNewItem;
				this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel.RegisterPropertyChangeHandler<String>
					(nameof(Game.PlayerTestItem.Test1), this.Set__TextMeshProUGUI__Sheet_Name___text);
				this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel.RegisterPropertyChangeHandler<String>
					(nameof(Game.PlayerTestItem.Test2), this.Set__TextMeshProUGUI__Sheet_Password___text);
				this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0.ViewModel.RegisterPropertyChangeHandler<System.String>
					(nameof(Game.PlayerTestItem.Test2), this.Set__TextMeshProUGUI__Sheet_BtnComfirm_Text___text_Related_Test2);
				this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel.RegisterPropertyChangeHandler<String>
					(nameof(Game.PlayerTestItem.Test2), this.Set__TMP_InputField__Sheet_InputField___text);
				// 双向绑定
				this.__TMP_InputField__Sheet_InputField__.onValueChanged.RemoveListener(this.Event__TMP_InputField__Sheet_InputField___onValueChanged);
				this.__TMP_InputField__Sheet_InputField__.onValueChanged.AddListener(this.Event__TMP_InputField__Sheet_InputField___onValueChanged);

				this.__Button__Sheet_BtnComfirm__.onClick.RemoveListener(this.Event__Button__Sheet_BtnComfirm___onClick);
				this.__Button__Sheet_BtnComfirm__.onClick.AddListener(this.Event__Button__Sheet_BtnComfirm___onClick);

				this.ViewModel = rCellData.ViewModel;
			}

			this.Index = nIndex;

			var rItem = rCellData.ViewModel as Game.PlayerTestItem;
			this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel.Test1 = this.__TextMeshProUGUI__Sheet_Name__PlayerTestItemListTemlate_0.ViewModel.Test1;
			this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel.Test2 = this.__TextMeshProUGUI__Sheet_Password__PlayerTestItemListTemlate_0.ViewModel.Test2;
			this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel.Test2 = this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel.Test2;
		}

		private void Set__TextMeshProUGUI__Sheet_Name___text(String rValue)
		{
			this.__TextMeshProUGUI__Sheet_Name__.text = rValue;
		}

		private void Set__TextMeshProUGUI__Sheet_Password___text(String rValue)
		{
			this.__TextMeshProUGUI__Sheet_Password__.text = rValue;
		}

		private void Set__TextMeshProUGUI__Sheet_BtnComfirm_Text___text_Related_Test2(System.String rValue)
		{
			var rRelatedValue = this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__PlayerTestItemListTemlate_0.ViewModel.Test1RelatedTest2;
			this.__TextMeshProUGUI__Sheet_BtnComfirm_Text__.text = rRelatedValue;
		}

		private void Set__TMP_InputField__Sheet_InputField___text(String rValue)
		{
			this.__TMP_InputField__Sheet_InputField__.text = rValue;
		}

		private void Event__TMP_InputField__Sheet_InputField___onValueChanged(System.String rArg0)
		{
			this.__TMP_InputField__Sheet_InputField__PlayerTestItemListTemlate_0.ViewModel.Test2 = rArg0;
		}

		private void Event__Button__Sheet_BtnComfirm___onClick()
		{
			var rPlayerView = this.gameObject.GetComponentInParent<PlayerView>();
			rPlayerView?.PlayerViewController?.OnListBtnComfirmClicked(this.Index);
		}

	}
}
