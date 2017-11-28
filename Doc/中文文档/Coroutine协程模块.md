# Coroutine协程管理
* （题外话） 尽管网上很多大佬并不推荐在Unity中使用协程，但是不可否认的是在U3D中使用协程来做异步处理可以让代码变得非常简洁好看。特别是在和U3D的资源加载一起配合使用的时候真的很方便。所以我还是非常推崇在U3D中使用协程来做异步处理的。
* 目前项目中的异步操作已经全部替换为async await模式了，支持Unity Coroutine和async await相互混用，自由转变。

## 协程模块支持的功能
* 协程使用统一管理，让携程的启动不再依赖MonoBehaviour。
* 实现类似 WWW/AssetBundleRequest等带自定义参数返回的协程对象，以简化使用协程的代码结构。
* 支持在热更新端使用协程。

## 模块提供的API
* ![coroutine_1](https://github.com/winddyhe/knight/blob/master/Doc/res/images/coroutine_1.png)
* 框架中提供的协程管理，可以在任意位置启动一个协程操作，不受MonoBehaviour的约束。
```C#
CoroutineManager.cs
public CoroutineHandler StartHandler(IEnumerator rIEnum)	// 开始一个可控制(停掉)的协程
public Coroutine Start(IEnumerator rIEnum)					// 开始一个普通协程
public void Stop(CoroutineHandler rCoroutineHandler)		// 停止一个协程
```
* 自定义参数返回的携程对象
```C#
public class LoaderRequest : CoroutineRequest<LoaderRequest>
{
    public Object obj;
    public string path;

    public LoaderRequest(string rPath)
    {
        this.path = rPath;
    }
}

public class CoroutineLoadTest
{
    public LoaderRequest mRequest;

    private LoaderRequest Load_Async(string rPath)
    {
        LoaderRequest rRequest = new LoaderRequest(rPath);
        mRequest = rRequest;
        rRequest.Start(Load(rRequest));
        return rRequest;
    }

    private IEnumerator Load(LoaderRequest rRequest)
    {
        yield return new WaitForSeconds(1.0f);
        rRequest.obj = new GameObject(rRequest.path);
        Debug.LogFormat("Create GameObject: {0}", rRequest.path);
    }

    public IEnumerator Loading(string rPath)
    {
        yield return Load_Async(rPath);
    }
}

public class CoroutineTest : MonoBehaviour
{
    IEnumerator Start()
    {
        CoroutineManager.Instance.Initialize();
        yield return CoroutineManager.Instance.Start(Start_Async());
    }

    IEnumerator Start_Async()
    {
        CoroutineLoadTest rTest = new CoroutineLoadTest();
        yield return CoroutineManager.Instance.Start(rTest.Loading("Test1"));
        yield return CoroutineManager.Instance.Start(rTest.Loading("Test2"));
        Debug.Log("Done.");
    }
}
```

* async await异步模式
```C#
public class Init : MonoBehaviour
{
    public string HotfixABPath = "";
    public string HotfixModule = "";
    
    async void Start()
    {
        CoroutineManager.Instance.Initialize();
	wait Start_Async();
    }

    private async Task Start_Async()
    {
        await ABPlatform.Instance.Initialize();
        await ABUpdater.Instance.Initialize();

        await HotfixManager.Instance.Load(this.HotfixABPath, this.HotfixModule);

        await HotfixGameMainLogic.Instance.Initialize();

        Debug.Log("End init..");
    }
}

// 直接await Assetbundle.LoadFromFileAsync API
rAssetLoadEntry.CacheAsset = await AssetBundle.LoadFromFileAsync(rAssetLoadUrl);

// 直接await Assetbundle.LoadAssetAsync API
rRequest.Asset = await rAssetLoadEntry.CacheAsset.LoadAssetAsync(rRequest.AssetName);
```

## 需要注意的问题
* ILRuntime库目前不支持在foreach中使用yield return xxx;操作。
```C#
// 错误的用法
foreach (var rPair in rGameStageList)
{
	yield return rPair.Value.Run_Async();
}

// 正确的用法：目前只能转成for循环来执行协程
var rGameStageList = new List<KeyValuePair<int, GameStage>>(this.gameStages);
for (int i = 0; i < rGameStageList.Count; i++)
{
    yield return rGameStageList[i].Value.Run_Async();
}
```
* 目前在热更新端还不支持直接继承CoroutineRequest类，模板类的继承在ILRuntime中支持得不是很好。在热更新端可以直接创建一个类来缓存CoroutineHandler对象来模拟实现带参数的协程对象。具体用法参见热更新DLL中的ActorCreater.cs文件。











