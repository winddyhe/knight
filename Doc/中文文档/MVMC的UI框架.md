# WindUI模块
* WindUI是一个轻量级的MVVM UI框架，实现数据逻辑和显示逻辑完全分离。
* 开发者只需要关心数据和界面的绑定，以及实现数据逻辑即可自动驱动UI的显示逻辑。
* 提供了一个基于UGUI的图集管理模块。UIAtlasManager
* 提供了一系列的常用的UGUI扩展组件。

## UI数据绑定
### ViewModel容器(ViewModelContainer)
* ![ui_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/ui_1.png)
* 每一个UI预制件中拥有一个ViewModel容器脚本(ViewModelContainer)，他的ViewModelClass变量在热更新端对应一个类，在实例化这个UI的时候会创建一个ViewModelContaner对象出来。
* ViewModels列表，存储了多个ViewModelDataSource对象。
* EventBindings列表，储存多个事件绑定脚本对象。

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
* 热更新端逻辑类、属性变量‘方法加上DataBinding属性标签，就可以在Inspector下添加。
* LoginView中的变量添加HotfixBinding属性标签，就可以自动绑定ViewModelContainer中对应的ViewModel值。

```C#
    [DataBinding]
    public class LoginViewModel : ViewModel
    {
        private string      mAccountName    = "Test111";
        private string      mPassword       = "xxxxxxx";

        [DataBinding]
        public  string      AccountName
        {
            get { return mAccountName;  }
            set
            {
                mAccountName = value;
                this.PropChanged("AccountName");
            }
        }

        [DataBinding]
        public  string      Password
        {
            get { return mPassword;     }
            set
            {
                mPassword = value;
                this.PropChanged("Password");
            }
        }
    }
	
	public class LoginView : ViewController
    {
        [HotfixBinding("Login")]
        public LoginViewModel   ViewModel;

        protected override void OnOpening()
        {
        }
        
        protected override void OnUpdate()
        {
        }

        [DataBinding]
        private void OnBtnButton_Clicked()
        {
            ViewManager.Instance.Open("KNListTest", View.State.Dispatch);
        }
    }
```