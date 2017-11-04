# Pomelo服务器集成
* 框架中使用的Pomelo服务器不是目前最新的版本。使用的1.2.2版本。
* 为了能够方便的运行框架Demo，目前框架中已经屏蔽掉服务器的登陆流程了。
* Pomelo的客户端插件并不是原版的，我进行过改造，将里面的Json解析库替换成了WindJson，方便和框架其他逻辑能够完美的契合。
* 目前Pomelo的服务器只做了登陆功能，由于该服务器是nodejs，而且现在并没有维护这个开源库了，所以本框架在后期也不打算继续在该服务器上做游戏逻辑的开发了。将考虑接入一个.NetCore的服务器，以实现双端使用C#开发。如果有朋友有比较好的.NetCore游戏服务器，欢迎推荐。

## 本地搭建Pomelo服务器
* 由于版本的关系，Pomelo服务器game-server中的node_modules并不直接用npm下载下来，在knight/Server/pomelo-knight文件夹中有一个node_modules.7z压缩包，将其解压到game-server目录中即可。
* 框架中使用mongodb作为数据库，在knight/Server/mongodb-win32-i386-2.0.4文件夹中点击start-mongodb-knight.bat文件运行mongodb数据库。
* 使用vscode打开game-server，可以调试运行pomelo服务器。也可直接用命令行运行服务器。

* 如果要开启服务器登陆流程在热更DLL的LoginView.cs中开启服务器登陆代码1、屏蔽掉直接创建角色的代码2即可。
* ![Pomelo_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/pomelo_1.png)
