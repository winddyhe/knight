# knight
支持一波996.ICU
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>

Knight是一个基于Unity引擎的游戏GamePlay框架，提供一些简单易用的游戏框架接口，目的让开发者更加专注于游戏内容的开发。

它包含了一个完整的资源管理模块（打包、下载、加载、版本管理），一个基于hybridclr的C#热更模块，一个基于MVMC的UI框架以及其他基础功能的支持。

本框架目前以提升性能和易用性为目的，已经重构了大部分以前的设计。目前使用的Unity版本为Unity6000.0.26f1。
目前各个模块采用Package的方式进行分隔，并使用PackageManager来管理他们，以实现使用时可随时插拔。

  ![knight的框架结构](https://github.com/winddyhe/knight/blob/master/Doc/res/images/img_1.png)

### 编辑器运行游戏
* 使用编辑器模拟Assetbundle资源模式运行，无需打包Assetbundle。找到路径Assets/Game.Editor/Assetbundle/ABSimulateConfig.asset文件，选中他勾选IsDevelopMode、IsHotfixABMode、和SimulateType选择Everything。

  ![knight编辑器模拟模式](https://github.com/winddyhe/knight/blob/master/Doc/res/images/img_2.png)
* 打开Assets/Game/Scene/Game.unity场景，点Play运行游戏Demo。

### 主要功能介绍
* [框架结构](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/%E6%A1%86%E6%9E%B6%E7%BB%93%E6%9E%84.md)
* [Assetbundle资源管理](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/Assetbundle%E8%B5%84%E6%BA%90%E7%AE%A1%E7%90%86.md)
* [hybridclr热更新](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/hybridclr%E7%83%AD%E6%9B%B4%E6%96%B0.md)
* [MVMC的UI框架](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/MVMC%E7%9A%84UI%E6%A1%86%E6%9E%B6.md)
* [GameConfig配置](https://github.com/winddyhe/knight/blob/master/Doc/%E4%B8%AD%E6%96%87%E6%96%87%E6%A1%A3/GameConfig%E9%85%8D%E7%BD%AE.md)

### 插件(感谢以下插件和框架对knight的底层功能的支持)
* hybridclr: 是一个特性完整、零成本、高性能、低内存的Unity全平台原生c#热更新解决方案。 地址：https://github.com/focus-creative-games/hybridclr
* NaughtyAttributes: 一个脚本Inspector UI扩展库，通过Attribute标签来实现的Editor扩展。地址：https://github.com/dbrizov/NaughtyAttributes
* UniTask: 一个为Unity集成的一个0GC的高性能的async/await库。地址：https://github.com/Cysharp/UniTask
* protobuf: google开源的一个二进制序列化库，用于网络协议解析。地址：https://github.com/protocolbuffers/protobuf
* TouchSocket: 是一个功能强大且易于使用的.NET 网络通信框架。支持解决TCP黏包分包问题和UDP大数据包分片组合问题。框架支持多种协议模板，快速实现固定包头、固定长度和区间字符等数据报文解析。地址：https://github.com/RRQM/TouchSocket
* Nino: 一个高性能的二进制序列化库。地址：https://github.com/JasonXuDeveloper/Nino
* FancyScrollView: 一个UI无限循环列表库。地址：https://github.com/setchi/FancyScrollView
* UnityIngameDebugConsole: 一个用于运行时显示log的工具。地址：https://github.com/yasirkula/UnityIngameDebugConsole

### 联系方式
* Email: hgplan@126.com 
* QQ群: 651543479
