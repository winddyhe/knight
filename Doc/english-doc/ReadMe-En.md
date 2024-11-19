# knight
Support the 996.ICU movement
<a href="https://996.icu"><img src="https://img.shields.io/badge/link-996.icu-red.svg" alt="996.icu"></a>

[中文](https://github.com/winddyhe/knight/blob/master/README.md)

* Knight is a GamePlay framework based on the Unity engine, offering simple and user-friendly game framework interfaces, with the aim of enabling developers to focus more on the development of game content.
* It includes a complete resource management module (packaging, downloading, loading, version management), a C# hot update module based on hybridclr, a UI framework based on MVMC, as well as support for other basic functionalities.
* The current framework aims to enhance performance and usability, and has undergone a major restructuring of previous designs. The currently used version of Unity is Unity6000.0.26f1.
* Currently, each module is separated using the Package approach and managed by the PackageManager to achieve plug-and-play functionality at runtime.
  
  ![Framework Architecture of knight](https://github.com/winddyhe/knight/blob/master/Doc/res/images/img_1.png)

### Editor runs game
* Use the editor to simulate the Assetbundle resource mode without packaging the Assetbundle. Locate the file Assets/Game. Editor/Assetbundle/ABSimulateConfig.asset, select it, and check IsDevelopMode, IsHotfixABMode, and SimulateType to select Everything.
  
  ![Knight Editor Simulation Mode](https://github.com/winddyhe/knight/blob/master/Doc/res/images/img_2.png)
* Open the Assets/Game/Scene/Game.unity scene, and click "Play" to run the game demo.

### Introduction to main functions
* [Framework Architecture](https://github.com/winddyhe/knight/blob/master/Doc/english-doc/FrameworkArchitecture.md)
* [Assetbundle Management](https://github.com/winddyhe/knight/blob/master/Doc/english-doc/AssetbundleManagement.md)
* [hybridclr hot update](https://github.com/winddyhe/knight/blob/master/Doc/english-doc/Hybridclr-HotUpdate.md)
* [MVMC's UI Framework](https://github.com/winddyhe/knight/blob/master/Doc/english-doc/MVMC-UIFramework.md)
* [GameConfig](https://github.com/winddyhe/knight/blob/master/Doc/english-doc/GameConfig.md)

### Plugins (Thanks to the following plugins and frameworks for supporting the underlying functionality of knight)
* hybridclr: It is a feature-complete, zero-cost, high-performance, low-memory Unity full-platform native C# hot update solution. Address: https://github.com/focus-creative-games/hybridclr
* NaughtyAttributes: A script Inspector UI extension library, implementing Editor extensions through Attribute tags. Address: https://github.com/dbrizov/NaughtyAttributes
* UniTask: A high-performance async/await library with 0GC integrated for Unity. Address: https://github.com/Cysharp/UniTask
* Protobuf: A binary serialization library open-sourced by Google, used for network protocol parsing. Website: https://github.com/protocolbuffers/protobuf
* TouchSocket: It is a powerful and easy-to-use .NET network communication framework. It supports solving the issues of TCP packet coalescing and fragmentation, as well as UDP large packet slicing and composition. The framework supports multiple protocol templates, enabling rapid implementation of data packet parsing for fixed headers, fixed lengths, and range characters. Website: https://github.com/RRQM/TouchSocket
* Nino: A high-performance binary serialization library. Address: https://github.com/JasonXuDeveloper/Nino
* FancyScrollView: A UI infinite scrolling list library. Address: https://github.com/setchi/FancyScrollView
* UnityIngameDebugConsole: A tool for displaying logs during runtime. Address: https://github.com/yasirkula/UnityIngameDebugConsole

### Contact Information
* Email: hgplan@126.com 
* QQ Group: 651543479
