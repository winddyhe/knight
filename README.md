# knight
Knight是一个基于Unity5.x引擎的游戏GamePlay框架，提供一些简单易用的游戏框架接口，目的让开发者更加专注于游戏内容的开发。同时在框架基础上提供一些不同游戏类型的Demo。


## 主要功能
* WindJson: 
    * 标准的Json格式解析
    * 方便的从object/jsonstring/jsonnode三者之间相互转化
    * 兼容重复的逗号，分号
    * 支持枚举类型、true/false关键字的识别
    * 支持 // /**/ 注释的识别

* 完整的Assetbundle资源打包工具与加载模块
    * 封装Unity5.x的打包系统
    * 统一资源加载接口，自动根据依赖项加载资源
    * TODO: UI资源加入打包系统

* Coroutine协程管理
    * 实现类似 WWW/AssetBundleRequest等带自定义参数返回的协程对象，以简化使用协程的代码结构

* Pomelo服务器集成
    * Pomelo是一个基于Nodejs的游戏服务器，集成它的客户端模块到游戏框架中去
    * 提供服务器消息发送和接收的接口
    * 服务器登录流程
    * 数据库使用MongoDB
    * TODO: 服务器相关功能，如位置同步、伤害计算同步等等

* WindUI: 
    * UI资源加载和UI对象的管理
    * 封装可复用的UI控件

* ExcelReader
    * 支持Excel表格的读取，转换为Json格式的字符串

* MultiScene: 地形分块
    * 将一个大地形资源分成小块地形
    * 根据角色位置按照一定的策略加载和卸载不同的地形块

* Knight游戏
    * 一个MMORPG的游戏Demo，开发中

## 插件
* PomeloClient: Pomelo提供的U3D客户端插件，地址：https://github.com/NetEase/pomelo-unityclient-socket
* ILRuntime: 一个使用C#编写的用来运行C#程序的虚拟机，用来实现热更新机制，地址：https://github.com/Ourpalm/ILRuntime

## 计划 2017.3.16
* 使用ILRuntime做热更新，把全部的游戏逻辑放到ILRuntime端去。实现全逻辑的热更新机制。
* 整合官方的AssetbundleBrowser，实现资源管理模块Editor和非Editor的加载接口统一，可以自由切换。

## 联系方式
Email: hgplan@126.com
QQ: 532815352

## 版本记录
### v0.1.0 [2016/07/28]
1. 初次建立
