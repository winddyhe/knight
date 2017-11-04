# UI管理模块(WindUI)
* WindUI采用MVC结构进行构建，UI的管理和UI的数据完全分离，因此可以很方便的切换底层UI到底使用NGUI、UGUI还是其他的UI系统。
* UI的管理逻辑全部在热更新端，在主工程端只有UI资源加载器UIAssetLoader、UI数据容器ViewContainer。

## UI数据容器ViewContainer
* ViewContainer继承至HotfixMBContainer，对HotfixMBContainer类不清楚的请看《ILRuntime热更新模块》篇。仅使用它来获取到UIPrefab中的数据对象引用，以供热更新端的UI逻辑使用。
* ![ui_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_1.png)

## 热更DLL中的UI管理
* ![ui_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_2.png)
* 所有的UI逻辑类都将继承自TViewController，同样的使用HotfixBinding和HotfixBindingEvent来绑定数据和事件，让逻辑代码不会被其他非逻辑代码污染，让代码变得更加精简。

```C#
namespace KnightHotfixModule.Test.UI
{
	public class UILoginTest : TViewController<UILoginTest>
	{
		[HotfixBinding("AccountInput")]
		public InputField       AccountInput;
		[HotfixBinding("PasswordInput")]
		public InputField       PasswordInput;

		public override void OnInitialize()
		{
			Debug.LogError("UILoginTest.Initialize..." + this.Objects.Count);
		}

		public override void OnOpening()
		{
			Debug.LogError("OnOpening: " + this.mIsOpened);
		}

		public override void OnClosing()
		{
			Debug.LogError("OnClosing: " + this.mIsClosed);
		}

		[HotfixBindingEvent("LoginBtn", EventTriggerType.PointerClick)]
		private void OnButton_Clicked(UnityEngine.Object rObj)
		{
			Debug.LogError("Button Clicked..." + this.AccountInput.text + ", " + this.PasswordInput.text);
		}
	}
}
```

* 打开一个UI，支持异步打开一个UI
```C#
yield return ViewManager.Instance.OpenAsync("KNLogin", View.State.dispatch);
```

* UI支持三种状态
	* fixed: 固定的，不会被其他的UI切换掉的
	* overlap: 叠层在一起的，加载一个新UI不会被切换掉
	* dispatch: 可切换的，加载一个新UI会被切换掉

