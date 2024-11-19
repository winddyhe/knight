# Assetbundle资源管理

## 资源打包
![改造后的AssetbundleBrowser](https://github.com/winddyhe/knight/blob/master/Doc/res/images/assetbundlebrowser_improved.png)
* 自定义资源标签配置工具，给资源在逻辑上分成不同的类别，通过配置能够很方便的进行资源分类。
* 在项目后期优化阶段将体会到资源区分类别的好处，这样能够让不同的功能、不同模块的资源之间相互没有关联。能够方便的控制资源包的粒度，减少资源包的依赖项。
* 资源分类中需要注意的一个问题是处理好在不同的AB包中重用的资源，可以将他们分类成独立的资源包。

## 资源的更新
### 更新下载流程
* 在Unity游戏的资源更新过程中，资源包将会存在于三个地方：StreamingAssets空间（直接包含到游戏包中的资源）、Server空间（服务器上用来做热更新的资源）、Persitent空间（从服务器下载的下来的资源）。
* 在框架中在资源打包的时候重新为Assetbundle包生成了更多信息的一个版本文件ABVersion.Bin和版本文件的MD5码，ABVersion_MD5.Bin。在ABVersion.Bin版本文件中，比Unity生成的版本文件多了资源包的大小Size信息和版本号Version信息。
* 通过以上的MD5码和版本号信息，就能轻松的判断哪些资源包需要更新到Persitent空间中去。
* 下载资源使用API是Unity提供的UnityWebRequest。
* 资源更新流程如下：
1. 加载Persistent空间下的ABVersion_MD5.Bin文件。
2. 下载Server空间下的ABVersion_MD5.Bin文件。
3. 比较Persistent和Server下的MD5码是否相等？如果不相等转4，否则转9退出下载流程。
4. 下载Server空间下的ABVersion.Bin版本信息文件。
5. 加载Persistent空间下的ABVersion.Bin版本信息文件。
6. 比较Server和Persistent下的版本信息文件，将需要更新的资源包文件记录到一个List中。（需要更新的资源包的条件：a. Server上单个资源包版本号大于Persistent空间单个资源包版本号。 b. 只有Server上存在的资源包）
7. 遍历需要更新的资源包文件列表，从服务器上下载资源包到Persistent空间中，下载完一个资源包更新一次Persistent空间的版本信息文件ABVersion.Bin。
8. 整个下载过程完成后，将Server的版本MD5码保存到Persistent空间中，如果下载过程终端则不保存版本MD5码。
9. 最后将比较Persitent空间和StreamingAssets空间中的版本文件最终生成用于资源加载的版本信息数据对象ABLoaderVersion。（比较两个空间下单个资源包版本号较大的作为最终使用的资源包路径。）
10. 完成更新下载流程。

### Server空间上的增量更新包的生成
* 每次打包资源的时，将会资源打包的增量信息，保存在资源包文件夹的History中。
* 在打包工具页面提供了一个增量资源包Build功能，选择BuildPackageType为HotfixPackage，用来提取从某个版本开始的所有的增量资源包。

  ![AssetbundleHistory](https://github.com/winddyhe/knight/blob/master/Doc/res/images/assetbundle_histroy.png)

## 资源的加载
* 框架中建议采用的资源加载方式是 LZ4 + Assetbundle.LoadFromFileAsync。这种加载方案目前是AssetBundle加载中最优的，能够最大程度的节省加载时间和内存分配。
* 所有的资源包版本信息都存放在ABLoaderVersion对象中，这个数据对象通过更新流程生成。
* 框架中采用协程处理资源包的异步加载。通过递归调用来加载资源包的依赖项。
* 框架中Cache Assetbundle对象 + 引用计数 + Unload(true)的方式进行资源加载管理，这种方式能够明确的卸载掉某个资源。
* 统一底层的资源加载、卸载接口：
```
IAssetLoader Interface
public AssetLoaderRequest<T> LoadAssetAsync<T>(string rAssetPath, string rAssetName, bool bIsSimulate) where T : Object;
public AssetLoaderRequest<T> LoadAllAssetAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object;
public AssetLoaderRequest<T> LoadSceneAsync<T>(string rAssetPath, string rAssetName, LoadSceneMode rSceneMode, bool bIsSimulate) where T : Object;
public AssetLoaderRequest<T> LoadAllSceneAsync<T>(string rAssetPath, bool bIsSimulate) where T : Object;
public void Unload<T>(AssetLoaderRequest<T> rRequest) where T : Object;
```
* 提供了在Editor中使用UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName和UnityEditor.AssetDatabase.LoadMainAssetAtPath API来模拟资源包的加载。这样在Editor中资源发生改变了就不再需要重新Build资源包就能够得到正确的结果了。
* 通过设置文件Assets/Game.Editor/Assetbundle/ABSimulateConfig.asset文件，选中他勾选IsDevelopMode、IsHotfixABMode、和SimulateType选择不同类型的资源模拟。

  ![Assetbundle模拟模式](https://github.com/winddyhe/knight/blob/master/Doc/res/images/img_2.png)
