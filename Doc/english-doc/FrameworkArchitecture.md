# The framework structure of Knight
## Framework Structure Diagram
![Framework structure of knight](https://github.com/winddyhe/knight/blob/master/Doc/res/images/knight_framework.png)

## Core Libraries
* The CoreLib layer primarily provides a series of basic libraries that are independent of the Knight framework. It mainly consists of two packages: Knight. Core and Knight. Core.Unity.
* This design separates the core library into two parts: one is a pure C# code encapsulation, and the other is a code encapsulation that includes Unity APIs. The main purpose of this is to meet the requirement of separating game logic from Unity, such as running combat logic on the server.

### Pool (Object Pool)
* Provides a generic object pool of TObjectPool<T> and an object pool of GameObjectPool.

### Async/Await (asynchronous)
* The asynchronous integration library of UniTask has been introduced. The framework layer and game logic have completely abandoned coroutine calls and adopted the Async/Await approach to handle asynchronous logic.
* It also provides mutual invocation and conversion between Coroutine and Async/Await.

### Localization (multi-language localization)
* Provides a basic logic encapsulation interface for multi-language localization, which can be used in conjunction with the configuration table GameConfig/LanguageTable to achieve multi-language switching.

### Event (Event System)
* Redesign the event system, utilizing the Class/Object paradigm to articulate: event type/event listener instance. An event class represents an event type, while an object instance of the class signifies an event listener binding.
* Construct and distribute event objects using template types, completely eliminating the GC allocation caused by data boxing and unboxing that was previously brought about by object parameter passing.

### TypeResolve (Type Redirection)
* Obtain the types in each module Assembly in the game in advance, so as to obtain the desired Type at any location without worrying about cross-domain issues related to type reflection.
* Available in both the editor and runtime.

### Code Generate (代码生成)
* Provide a convenient code generation interface to meet the needs of code generation tools at both the framework and logic layers.

### Basic Assist Development Library (Assist)
* Provide a series of extension methods for basic classes to simplify coding.
* Provides a series of basic tool helper classes (reflection, path parsing, MD5 calculation, random number, log printing, singleton template).
* Provide a manager for asynchronously instantiated objects, which can be used to asynchronously instantiate GameObject objects.
* Provides a WebRequest tool that can be used to download files and content from the network.

## Third-party library for plugins (ThirdLib)
* hybridclr: It is a feature-complete, zero-cost, high-performance, low-memory Unity full-platform native C# hot update solution. Website: https://github.com/focus-creative-games/hybridclr
* NaughtyAttributes: A script Inspector UI extension library, implementing Editor extensions through Attribute tags. Address: https://github.com/dbrizov/NaughtyAttributes
* UniTask: a high-performance async/await library with 0GC integrated for Unity. Address: https://github.com/Cysharp/UniTask
* Protobuf: a binary serialization library open-sourced by Google, used for parsing network protocols. Website: https://github.com/protocolbuffers/protobuf
* TouchSocket: It is a powerful and easy-to-use .NET network communication framework, suitable for languages such as C#. Address: https://github.com/RRQM/TouchSocket
* Nino: A high-performance binary serialization library. Address: https://github.com/JasonXuDeveloper/Nino
* FancyScrollView: A UI infinite loop list library. Address: https://github.com/setchi/FancyScrollView
* UnityIngameDebugConsole: A tool for displaying logs during runtime. Address: https://github.com/yasirkula/UnityIngameDebugConsole

## Framework layer
* The Framework layer mainly includes:
	* Assetbundle resource management (Knight.Framework.Assetbundle)
	* Code hotfix (Knight.Framework.Hotfix)
	* Graphics rendering (Knight.Framework.Graphics)
    * Network and Protocol (Knight.Framework.Network/Knight.Framework.Protobuf)
    * Game save file (Knight.Framework.GameSave)
    * MVMC's UI framework (Knight.Framework.UI)
    * Serialization (Knight.Framework.Serializer)
    * Shader variant management (Knight.Framework.ShaderVariant)
* This layer depends on CoreLib and ThirdLib

### Assetbundle resource management (Knight.Framework.Assetbundle)
* Custom resource tag configuration tool, which determines which resources need to be packaged and the packaging strategy based on the configuration
* Unify resource loading interfaces, automatically load resources based on dependencies
* Customize the resource version information file to implement the resource package download, update, and version management processes
* Unify the loading interfaces for the resource management module Editor Simulate and non-Editor, allowing for free switching based on different resource types
* Disguise some resources that do not require assetbundle packaging and add them to the version information file, such as video resources, which can be updated and downloaded using the same logic as assetbundle
* Perform offset encryption on the file header of the assetbundle during resource packaging, and decrypt it during reading

### Code Hotfix (Knight.Framework.Hotfix)
* Implement full logic hot update for C# using hybridclr
* Hot-update DLLs are added to Assetbundle resource packs for management
* With this method of code hot update, you no longer need to worry about the special syntax for hot-updating code. You can simply write normal C# logic

### Graphics Rendering (Knight.Framework.Graphics)
* Provide detection and setting of graphics rendering levels (uncompleted, to be continued)
* Provide layer management for the URP pipeline camera stack, controlling the camera order for UI and scene rendering (not completed, to be continued)

### Network and Protocol (Knight.Framework.Network/Knight.Framework.Protobuf)
* Introduce TouchSocket as the underlying layer for network connection and data transmission, and encapsulate the interface for TCP connection and data transmission based on this library
* Introduce Protobuf as the data transmission format for network protocols, serializing and deserializing network data during transmission

### Game Save (Knight.Framework.GameSave)
* Provide a library for local game save files
* Set the variable to "Dirty" when calling it using static injection, and then detect which documents are "Dirty" in the Update process. Store them and use multithreading to write the content to local files
* The local file path written in Android is a hidden path, not visible to the outside world
* Perform XOR encryption on the file stream content of the written file
* Compatible with data versions, e.g., newly added fields in archived data can still be read correctly

### MVMC's UI Framework (Knight.Framework.UI)
* Provides a UGUI-based atlas management module, UIAtlasManager, which can obtain the corresponding Sprite or Texture2D object of an image based on its name
* Implement lightweight data binding based on MVVM, completely separating data logic from UI display logic, and providing a series of data and event binding scripts
* Provide a ViewController to control Model data and ViewModel data
* Provide a commonly used set of extended controls based on UGUI (to be gradually added in the future)
	* ImageReplace/RawImageReplace is used to dynamically replace an image in code
	* EmptGraphics is used to specify whether a blank UI area can accept click events
	* Gradient font gradient script
	* FancyScrollRect  Circular list
	* GrayGraphics  Grayed-out component
	* RedPoint: Little Red Point

### Serialization (Knight.Framework.Serializer)
* This module provides two serialization methods. One is based on Nino's binary serialization, which features high performance, automatic code generation, and compatibility with data versions
* Another approach is pure binary serialization, implemented through code generation
* Currently, both are used in the framework, and we will consider replacing them all with Nino's serialization in the future

### Shader Variant Management (Knight.Framework.ShaderVariant)
* This module provides the function of Shader variant management, which can globally collect used Shader variants and incrementally update new Shader variants

## Editor Tools Expanding
### Assetbundle Build Tools
* A tool for configuring and building resource bundles using UIElements, designed to manage and package asset bundles

### Excel Reader
* Exporting an Excel table into a Json-formatted data configuration and C# configuration code

### In-game Debug Console
* A third-party library used for viewing logs during game runtime

### Code Generator
* Code generation tool, capable of generating serialization code, ViewModel code for configurations and protocols, and more

### Auto Build Pipeline
* Automatically build the version pipeline, integrate the version building process, and implement tools for automatic version building (uncompleted, to be continued)

## Game Section
* The goal of this framework is to write all the game logic in the hot-update DLL.
* The game is mainly composed of two parts: the hot update module and resources.

### Resources
* GameAssets are the original assets, which will be packaged into Assetbundle packages for use in games.
* Assetbundle Packages are the original resource packages used in games.

### Hotfix Module for Game Logic
* Hotfix Core Library
* GUI Logic, referring to the GUI logic part of the game
* Battle Logic, referring to the combat logic aspect of the game