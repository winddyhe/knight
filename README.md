# knight
Knight是一个基于Unity引擎的游戏GamePlay框架，提供一些简单易用的游戏框架接口，目的让开发者更加专注于游戏内容的开发。

它包含了一个完整的资源管理模块（打包、下载、加载、版本管理），一个基于ILRuntime的C#热更模块，一个基于MVVM的UI框架（支持热更新）以及其他基础功能的支持。

本框架将会持续更新，后期会不断修改和完善框架中的内容。目前使用最新的Unity版本为Unity2018.2.2f1

### 运行游戏
* 运行菜单Tools/Assetbundle/Assetbundle Build命令，构建Assetbundle资源包。
* 打开Assets/Game/Scene/Game.unity场景，点Play运行游戏Demo。

### 主要功能介绍
* [框架结构](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%A1%86%E6%9E%B6%E7%BB%93%E6%9E%84.md)
* [Assetbundle资源模块](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/Assetbundle%E8%B5%84%E6%BA%90%E6%A8%A1%E5%9D%97.md)
* [ILRuntime热更新模块](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/ILRuntime%E7%83%AD%E6%9B%B4%E6%96%B0%E6%A8%A1%E5%9D%97.md)
* [WindJson](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/WindJson.md)
* [WindUI](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/WindUI.md)
* [Coroutine协程模块](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/Coroutine%E5%8D%8F%E7%A8%8B%E6%A8%A1%E5%9D%97.md)
* [服务器集成](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%9C%8D%E5%8A%A1%E5%99%A8%E9%9B%86%E6%88%90.md)
* [游戏中的配置](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%B8%B8%E6%88%8F%E4%B8%AD%E7%9A%84%E9%85%8D%E7%BD%AE.md)

### 插件(感谢以下插件和框架对knight的底层功能的支持)
* ILRuntime: 一个使用C#编写的解释运行C# IL代码的库，用来实现热更新机制，地址：https://github.com/Ourpalm/ILRuntime
* ET: 一个包含了分布式的.Net Core服务器的双端unity游戏框架。knight用到了它的服务器部分。地址：https://github.com/egametang/ET
* NaughtyAttributes: 一个脚本Inspector UI扩展库，通过Attribute标签来实现的Editor扩展。地址：https://github.com/dbrizov/NaughtyAttributes

### 联系方式
* Email: hgplan@126.com 
