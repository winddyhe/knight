using UnityEngine;
using System.Collections;
using Framework.WindUI;
using Core;
using Framework.Hotfix;

namespace Test
{
    public class HotfixUITest : MonoBehaviour
    {
        IEnumerator Start()
        {
            CoroutineManager.Instance.Initialize();

            // 加载热更新代码资源
            yield return HotfixApp.Instance.Load("KnightHotfixModule");

            // 加载界面
            yield return UIManager.Instance.OpenAsync("HotfixLogin", View.State.dispatch);
        }
    }
}