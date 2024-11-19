# MVMC的UI框架
* 该框架是一个轻量级的基于MVVM数据绑定的UI框架，实现逻辑数据和显示数据完全分离。其中又融合了MVC数据和逻辑分离的思想，形成了一套独有的MVMC的UI框架。
* 开发者只需要关心数据和界面的绑定，以及实现逻辑数据即可自动驱动UI的显示。
* 在控制器ViewController中实现界面ViewModel和数据Model的交互逻辑，以实现数据和逻辑的分离。
* 针对UI的层级、打开关闭的处理，提供了一个ViewManager管理器，并加入缓存界面信息和回退的功能。
* 提供了一个基于UGUI的图集管理模块：UIAtlasManager。
* 提供了一系列的常用的UGUI扩展组件。

## UI数据绑定
### ViewModel容器(View)
* ![ui_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_1.png)
* 每一个UI预制件中拥有一个ViewModel容器脚本(继承View的脚本)，这个脚本是在预制件保存时自动生成的数据绑定的代码，彻底避免了通过反射的数据绑定方式。
* 在初始化预制件时，会调用View.Initialize(ViewController)和View.Bind()来进行绑定View控制器和ViewModel的数据绑定。

### ViewModel数据源(ViewModelDataSource)
* ![ui_2](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_2.png)
* 这个类用来关联数据和UI，同时他是存储在ViewModelContainer中的。
* 通过反射ViewModelPath来创建ViewModel对象。

### 单向绑定(MemberBindingOneWay)
* ![ui_3](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_3.png)
* ViewPath变量：自动找出当前节点下所有组件中暴露给外界的变量。
* ViewModelPath变量：根据ViewPath的类型自动找到我们自定义的ViewModel类中的变量。
* 通过这两个变量即可达到数据跟UI绑定的效果。

### 双向绑定(MemberBindingTwoWay)
* ![ui_4](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_4.png)
* EventPath变量：自动找出当前节点中所有组件中暴露出来的事件触发接口，通过事件绑定能够监听到组件的数值变化。
* View Path变量：自动找出当前节点下所有组件中暴露给外界的变量。
* ViewModelPath变量：根据ViewPath的类型自动找到我们自定义的ViewModel类中的变量。

### 事件绑定(EventBinding)
* ![ui_5](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_5.png)
* ViewEvent变量：自动找出当前节点中所有组件中暴露出来的事件触发接口，通过事件绑定能够监听到组件的数值变化。
* ViewModelMethod：自动找出ViewModel中的所有包含绑定标签的方法。

### 热更新逻辑端
* 热更新端逻辑类、属性变量方法加上DataBinding属性标签，就可以在Inspector下添加。
* 此外还提供了DataBindingReleated属性，可以方便的使用相对依赖变量，例如NameRelatedTest1变量，如果Name值变化了，NameRelatedTest1也会相应的通知到UI界面层的显示。
* PlayerViewController中的变量添加ViewModelKey属性标签，就可以自动绑定ViewModelContainer中对应的ViewModel值。
* 在PlayerViewController中标记某个方法的属性为DataBindingEvent，表示可以在Inspector下该方法可以作为事件绑定的方法。

```C#
    [DataBinding]
    public partial class PlayerViewModel : ViewModel    
    {
        // Data Binding
        [DataBinding]
        public string Name { get; set; }
        [DataBinding]
        public int Level { get; set; }
        [DataBinding]
        public int Exp { get; set; }
        [DataBinding]
        public int Coin { get; set; }
        [DataBinding]
        public string Password { get; set; }

        // Related Binding with single property
        [DataBindingRelated("Name")]
        public string NameRelatedTest1 => this.Name + "_RelatedTest1";

        // Related Binding with multiple properties
        [DataBindingRelated("Level, Exp")]
        public string LevelRelatedTest1 => (this.Level + this.Exp).ToString();
        
        // List Binding
        [DataBinding]
        public List<PlayerTestItem> TestList { get; set; }
    }
	
    public class PlayerViewController : ViewController
    {
        [ViewModelKey("Test1")]
        public PlayerViewModel Test1;
        [ViewModelKey("Test2")]
        public PlayerViewModel Test2;

        protected override async UniTask OnOpen()
        {
            await base.OnOpen();

            this.Test1.Name = "Test444.";
            this.Test1.Level = 100;
            this.Test1.Exp =200;
            this.Test1.Coin = 300;

            var rPlayerTestItems = new List<PlayerTestItem>();
            for (int i = 0; i < 100; i++)
            {
                var rPlayerTestItem = new PlayerTestItem();
                rPlayerTestItem.Test1 = $"Test1-{i}";
                rPlayerTestItem.Test2 = $"Test2-{i}";
                rPlayerTestItems.Add(rPlayerTestItem);
            }
            this.Test1.TestList = rPlayerTestItems;
        }

        protected override void OnClose()
        {
        }

        [DataBindingEvent(false)]
        public void OnBtnEnter_Clicked()
        {
            this.Test1.TestList[4].Test1 = this.Test1.Name;
            this.Test1.Exp = 400;
            LogManager.LogError($"OnBtnEnter_Clicked Test..{this.Test2.Password}, {this.Test1.Name}");
            ViewManager.Instance.Close(this.View.GUID);
            HotfixBattle.Instance.Initialize().WrapErrors();
        }

        [DataBindingEvent(true)]
        public void OnListBtnComfirmClicked(int nIndex)
        {
            LogManager.LogError($"OnListButton_Clicked Test..{nIndex}, {this.Test1.TestList[nIndex].Test1}");
        }
    }
```
