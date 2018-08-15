# ILRuntime热更新管理
* ILRuntime库是一个使用C#编写的解释运行C# IL代码的库，可以运行在IOS系统下。框架中使用ILRuntime来时实现基于C# DLL的代码逻辑热更新。
* ILRuntime库的地址：https://github.com/Ourpalm/ILRuntime。

## 热更新模块
* 热更新模块的代码在Assets/Framework/Hotfix下。
* 分别通过ILRuntime和反射调用实现了热更新功能，并且这两种模式可以通过一个开关一键切换。

## MonoBehaviour代理类HotfixMBContainer
* ILRuntime库并不推荐在热更新端直接使用MonoBehaviour。因此框架中提供了一个HofixMBContainer类来获取Prefab中对象的引用，同时代理执行MonoBehaviour类中的Awake Start Update Destroy Enable Disable等API。
* ![HofixMBContainer](https://github.com/winddyhe/knight/blob/master/Doc/res/images/hofix_1.png)

	* HofixClassName: 是热更新端DLL中的命名空间 + 类名。
	* HofixNeedUpdate: 这里为了做效率优化，提供了一个是否需要执行Update API的标记。
	* Obejects: Prefab中的对象引用。

## 热更逻辑模板类THofixMB
* 在热更DLL中有一个THotfixMB模板类，他是热更端所有逻辑脚本的基类。逻辑脚本通过继承他，并且重写他的虚函数来实现游戏逻辑。
* ![THotfixMB](https://github.com/winddyhe/knight/blob/master/Doc/res/images/hofix_2.png)

* 如上所示，框架提供了使用Attribute标签来实现数据绑定和事件绑定。HotfixBinding用来绑定HofixMBContainer中的引用对象，HotfixBindingEvent用来绑定Prefab产生的事件。

* Prefab中某个对象触发一个事件的方法：在该对象节点上挂一个HotfixEventTrigger的脚本，选择相应的事件类型，并设置产生事件的对象。
* ![THotfixMB](https://github.com/winddyhe/knight/blob/master/Doc/res/images/hofix_3.png)

* 以上的功能让热更新端具备正常Unity编程的能力。在此基础上可以在热更新端构建自己任意想要的游戏逻辑结构。
* 目前框架中游戏逻辑结构采用的是使用一个单个的总控制脚本来控制游戏逻辑的初始化和更新操作。脚本名字是GameLogicManager。