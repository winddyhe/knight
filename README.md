# knight
Knight是一个基于Unity引擎的游戏GamePlay框架，提供一些简单易用的游戏框架接口，目的让开发者更加专注于游戏内容的开发。同时在框架基础上提供一些不同游戏类型的Demo。

本框架目前以知识积累和功能预研为主，将会持续更新中。。
目前使用的Unity版本为Unity2017.1.1f1


## 主要功能
* ILRuntime全逻辑热更新
    * 使用ILRuntime实现C#的全逻辑热更新
	* 整合框架内其他模块和热更新模块的相互访问，尽可能的与正常Unity开发流程保持一致

* 完整的Assetbundle资源打包工具与加载模块
	* 整合官方的AssetbundleBrowser
    * 统一资源加载接口，自动根据依赖项加载资源
	* 实现资源包下载流程
	* 实现资源管理模块Editor Simulate和非Editor的加载接口统一，可以根据不同的资源类型自由切换

* WindJson Json解析库
    * 标准的Json格式解析
    * 方便的从object/jsonstring/jsonnode三者之间相互转化
    * 兼容重复的逗号，分号
    * 支持枚举类型、true/false关键字的识别
    * 支持 // /**/ 注释的识别

* 改造官方的MemoryProfiler工具
    * 添加对象查找的工具
    * 添加两次内存snap的比较工具

* Coroutine协程管理
	* 协程使用统一管理，让携程的启动不再依赖MonoBehaviour
    * 实现类似 WWW/AssetBundleRequest等带自定义参数返回的协程对象，以简化使用协程的代码结构

* Pomelo服务器集成
	* Pomelo是一个基于Nodejs的游戏服务器，集成它的客户端模块到游戏框架中去
    * 提供服务器消息发送和接收的接口
    * 服务器登录流程
    * 数据库使用MongoDB

* WindUI: 
	* 全热更新逻辑的MVC结构
    * UI资源加载和UI对象的管理
    * 封装可复用的UI控件

* ExcelReader
    * 支持Excel表格的读取，转换为Json格式的字符串

* Graphics
	* 提供一些常用的功能性Shader
	* 高效率的3D UI，Image和Text，可以自动合并批次，适用于血条和飘字
	
## 插件(感谢以下插件对本框架的底层功能的支持)
* PomeloClient: Pomelo提供的U3D客户端插件，地址：https://github.com/NetEase/pomelo-unityclient-socket
* ILRuntime: 一个使用C#编写的解释运行C# IL代码的库，用来实现热更新机制，地址：https://github.com/Ourpalm/ILRuntime


## 联系方式
Email: hgplan@126.com
QQ群:  651543479
