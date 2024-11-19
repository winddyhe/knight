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
	public class PlayerView : View
	{
		public PlayerViewController PlayerViewController;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_BtnEnter_Text__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_Text1__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_Text2__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_Text3__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_Text4__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0;

		public TMPro.TextMeshProUGUI __TextMeshProUGUI__UILogin_Window_Text_TMP__;
		public BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel> __TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0;

		public TMPro.TMP_InputField __TMP_InputField__UILogin_Window_InputField__;
		public BindableViewModel<TMPro.TMP_InputField, Game.PlayerViewModel> __TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0;

		public UnityEngine.UI.Button __Button__UILogin_Window_BtnEnter__;

		public Knight.Framework.UI.FancyScrollRectView __FancyScrollRectView__UILogin_ScrollView__;
		public BindableViewModel<Knight.Framework.UI.FancyScrollRectView, Game.PlayerViewModel> __FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0;

		public override ViewController ViewController => this.PlayerViewController;

		public override void Initialize(ViewController rViewController)
		{
			this.PlayerViewController = (PlayerViewController)rViewController;
			base.Initialize(rViewController);

			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__;
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_Text1__;
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_Text2__;
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_Text3__;
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_Text4__;
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0 = new BindableViewModel<TMPro.TextMeshProUGUI, Game.PlayerViewModel>();
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewObject = this.__TextMeshProUGUI__UILogin_Window_Text_TMP__;
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0 = new BindableViewModel<TMPro.TMP_InputField, Game.PlayerViewModel>();
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewObject = this.__TMP_InputField__UILogin_Window_InputField__;
			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0 = new BindableViewModel<Knight.Framework.UI.FancyScrollRectView, Game.PlayerViewModel>();
			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewObject = this.__FancyScrollRectView__UILogin_ScrollView__;
		}

		public override void Bind()
		{
			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test1"];
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test1"];
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test2"];
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test1"];
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test1"];
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test2"];
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test2"];
			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel = (Game.PlayerViewModel)this.ViewModels["Test1"];

			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_BtnEnter_Text___text);
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text1___text);
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TextMeshProUGUI__UILogin_Window_Text2___text);
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewModel.RegisterPropertyChangeHandler<System.String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text3___text_Related_Name);
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text4___text);
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TextMeshProUGUI__UILogin_Window_Text_TMP___text);
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel.RegisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TMP_InputField__UILogin_Window_InputField___text);
			// 双向绑定
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewObject.onValueChanged.AddListener(this.Event__TMP_InputField__UILogin_Window_InputField___onValueChanged);

			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel.RegisterPropertyChangeHandler<List<PlayerTestItem>>
				(nameof(Game.PlayerViewModel.TestList), this.Set__FancyScrollRectView__UILogin_ScrollView___FancyScrollRectView);
			// 事件绑定
			this.__Button__UILogin_Window_BtnEnter__.onClick.AddListener(this.Event__Button__UILogin_Window_BtnEnter___onClick);

			// 数据初始化
			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel.Name = this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel.Name;
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel.Name = this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel.Name;
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel.Password = this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel.Password;
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel.Name = this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel.Name;
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel.Password = this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel.Password;
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel.Password = this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel.Password;
			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel.TestList = this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel.TestList;
		}

		public override void UnBind()
		{
			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_BtnEnter_Text___text);
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text1___text);
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TextMeshProUGUI__UILogin_Window_Text2___text);
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewModel.UnregisterPropertyChangeHandler<System.String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text3___text_Related_Name);
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Name), this.Set__TextMeshProUGUI__UILogin_Window_Text4___text);
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TextMeshProUGUI__UILogin_Window_Text_TMP___text);
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel.UnregisterPropertyChangeHandler<String>
				(nameof(Game.PlayerViewModel.Password), this.Set__TMP_InputField__UILogin_Window_InputField___text);
			// 双向绑定
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewObject.onValueChanged.RemoveListener(this.Event__TMP_InputField__UILogin_Window_InputField___onValueChanged);

			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel.UnregisterPropertyChangeHandler<List<PlayerTestItem>>
				(nameof(Game.PlayerViewModel.TestList), this.Set__FancyScrollRectView__UILogin_ScrollView___FancyScrollRectView);
			// 事件绑定
			this.__Button__UILogin_Window_BtnEnter__.onClick.RemoveListener(this.Event__Button__UILogin_Window_BtnEnter___onClick);

			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__PlayerViewModelTest1_0.ViewModel = null;
			this.__TextMeshProUGUI__UILogin_Window_Text1__PlayerViewModelTest1_0.ViewModel = null;
			this.__TextMeshProUGUI__UILogin_Window_Text2__PlayerViewModelTest2_0.ViewModel = null;
			this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewModel = null;
			this.__TextMeshProUGUI__UILogin_Window_Text4__PlayerViewModelTest1_0.ViewModel = null;
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__PlayerViewModelTest2_0.ViewModel = null;
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel = null;
			this.__FancyScrollRectView__UILogin_ScrollView__PlayerViewModelTest1_0.ViewModel = null;

			this.PlayerViewController = null;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_BtnEnter_Text___text(String rValue)
		{
			this.__TextMeshProUGUI__UILogin_Window_BtnEnter_Text__.text = rValue;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_Text1___text(String rValue)
		{
			this.__TextMeshProUGUI__UILogin_Window_Text1__.text = rValue;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_Text2___text(String rValue)
		{
			this.__TextMeshProUGUI__UILogin_Window_Text2__.text = rValue;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_Text3___text_Related_Name(System.String rValue)
		{
			var rRelatedValue = this.__TextMeshProUGUI__UILogin_Window_Text3__PlayerViewModelTest1_0.ViewModel.NameRelatedTest1;
			this.__TextMeshProUGUI__UILogin_Window_Text3__.text = rRelatedValue;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_Text4___text(String rValue)
		{
			this.__TextMeshProUGUI__UILogin_Window_Text4__.text = rValue;
		}

		private void Set__TextMeshProUGUI__UILogin_Window_Text_TMP___text(String rValue)
		{
			this.__TextMeshProUGUI__UILogin_Window_Text_TMP__.text = rValue;
		}

		private void Set__TMP_InputField__UILogin_Window_InputField___text(String rValue)
		{
			this.__TMP_InputField__UILogin_Window_InputField__.text = rValue;
		}

		private List<FancyScrollCellData> __Temp___FancyScrollRectView__UILogin_ScrollView___DataList = new List<FancyScrollCellData>();
		private void Set__FancyScrollRectView__UILogin_ScrollView___FancyScrollRectView(List<Game.PlayerTestItem> rValueList)
		{
			if (rValueList == null)
			{
				this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.Clear();
				this.__FancyScrollRectView__UILogin_ScrollView__.UpdateData(this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList);
				return;
			}

			if (this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.Count > rValueList.Count)
			{
				this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.RemoveRange(rValueList.Count, this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.Count - rValueList.Count);
			}
			else
			{
				for (int i = this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.Count; i < rValueList.Count; i++)
				{
					this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList.Add(new FancyScrollCellData());
				}
			}
			for (int i = 0; i < rValueList.Count; i++)
			{
				this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList[i].ViewModel = rValueList[i];
			}
			this.__FancyScrollRectView__UILogin_ScrollView__.UpdateData(this.__Temp___FancyScrollRectView__UILogin_ScrollView___DataList);
		}

		private void Event__TMP_InputField__UILogin_Window_InputField___onValueChanged(System.String rArg0)
		{
			this.__TMP_InputField__UILogin_Window_InputField__PlayerViewModelTest2_0.ViewModel.Password = rArg0;
		}

		private void Event__Button__UILogin_Window_BtnEnter___onClick()
		{
			this.PlayerViewController.OnBtnEnter_Clicked();
		}

	}
}
