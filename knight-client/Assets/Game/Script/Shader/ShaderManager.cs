using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Knight.Core;
using UnityEngine;

namespace Game
{
    public class ShaderManager : TSingleton<ShaderManager>
    {
        private Dictionary<string, Shader> mShaderDict;

        private ShaderManager()
        {
        }

        public async UniTask Initialize(string rShaderAB)
        {
            this.mShaderDict = new Dictionary<string, Shader>();

            if (!AssetLoader.Instance.IsDevelopMode)
            {
                var rAssetsRequest = AssetLoader.Instance.LoadAllAssetAsync<Object>(rShaderAB, AssetLoader.Instance.IsSimulate(ABSimuluateType.Config));
                await rAssetsRequest.Task();
                if (rAssetsRequest.AllAssets != null)
                {
                    foreach (var rAsset in rAssetsRequest.AllAssets)
                    {
                        var rSVC = rAsset as ShaderVariantCollection;
                        if (rSVC != null)
                        {
                            rSVC.WarmUp();
                        }
                        else if (rAsset is Shader)
                        {
                            var rShader = rAsset as Shader;
                            if (rShader != null)
                            {
                                this.mShaderDict.Add(rShader.name, rShader);
                            }
                        }
                    }
                }
            }
            LogManager.Log("Load all shader complete: " + rShaderAB);
        }

        public bool TryGetShader(string rShaderName, out Shader rShader)
        {
            if (this.mShaderDict == null)
            {
                rShader = null;
                return false;
            }
            if (rShaderName.Equals("Hidden/InternalErrorShader"))
            {
                rShader = null;
                return false;
            }
            return this.mShaderDict.TryGetValue(rShaderName, out rShader);
        }
    }
}
