using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEngine;

namespace Knight.Framework.UI
{
    public class ViewAssetLoader : TSingleton<ViewAssetLoader>
    {
        private static string mViewAssetPrefix = "game/gui/prefabs/";
        private Dictionary<string, AssetLoaderRequest<GameObject>> mViewRequests;

        private ViewAssetLoader()
        {
            this.mViewRequests = new Dictionary<string, AssetLoaderRequest<GameObject>>();
        }

        public async UniTask<AssetLoaderRequest<GameObject>> LoadView(string rViewName)
        {
            if (this.mViewRequests.TryGetValue(rViewName, out var rRequest))
            {
                return rRequest;
            }
            var rViewABPath = $"{mViewAssetPrefix}{rViewName.ToLower()}.ab";
            rRequest = AssetLoader.Instance.LoadAssetAsync<GameObject>(rViewABPath, rViewName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
            this.mViewRequests.Add(rViewName, rRequest);
            await rRequest.Task();
            return rRequest;
        }

        public async UniTask<List<AssetLoaderRequest<GameObject>>> LoadViewGroup(IEnumerable<string> rViewGroup)
        {
            var rRequests = new List<AssetLoaderRequest<GameObject>>();
            var rUniTasks = new UniTask[rViewGroup.Count()];
            int i = 0;
            foreach (var rViewName in rViewGroup)
            {
                if (this.mViewRequests.TryGetValue(rViewName, out var rRequest))
                {
                    rUniTasks[i] = rRequest.Task();
                    continue;
                }
                var rViewABPath = $"{mViewAssetPrefix}{rViewName.ToLower()}.ab";
                rRequest = AssetLoader.Instance.LoadAssetAsync<GameObject>(rViewABPath, rViewName, AssetLoader.Instance.IsSimulate(ABSimuluateType.GUI));
                this.mViewRequests.Add(rViewName, rRequest);
                rRequests.Add(rRequest);
                rUniTasks[i] = rRequest.Task();
                i++;
            }
            await UniTask.WhenAll(rUniTasks);
            return rRequests;
        }

        public AssetLoaderRequest<GameObject> GetView(string rViewName)
        {
            if (!this.mViewRequests.TryGetValue(rViewName, out var rLoadRequest))
            {
                LogManager.LogError($"Cann't find view {rViewName} load request.");
                return null;
            }
            if (!rLoadRequest.IsLoadComplete)
            {
                LogManager.LogError($"View {rViewName} is not load complete.");
                return null;
            }
            return rLoadRequest;
        }

        public void UnloadView(string rViewName)
        {
            if (this.mViewRequests.TryGetValue(rViewName, out var rLoaderRequest))
            {
                AssetLoader.Instance.Unload(rLoaderRequest);
            }
            this.mViewRequests.Remove(rViewName);
        }

        public void UnloadViews(IEnumerable<string> rViewNames)
        {
            foreach (var rViewName in rViewNames)
            {
                this.UnloadView(rViewName);
            }
        }
    }
}
