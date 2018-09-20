using System;
using System.Collections.Generic;
using Knight.Core;
using System.Collections;
using System.Threading.Tasks;
using Knight.Framework.AssetBundles;
using UnityFx.Async;

namespace UnityEngine.UI
{
    public class UIAtlasManager : TSingleton<UIAtlasManager>
    {
        public class SpriteRequest : AsyncRequest<SpriteRequest>
        {
            public Sprite   Sprite;
            public Texture  Texture;

            public string   Path;
            public string   AssetName;

            public SpriteRequest(string rPath, string rAssetName)
            {
                this.Path = rPath;
                this.AssetName = rAssetName;
            }
        }

        public List<UIAtlas> Atlases;

        private UIAtlasManager()
        {
            this.Atlases = new List<UIAtlas>();
        }

        public async Task Load_Async(string rAtlasConfigABName)
        {
            bool bIsSimulate = ABPlatform.Instance.IsSumilateMode_GUI();

            var rLoadRequest = await ABLoader.Instance.LoadAllAssets(rAtlasConfigABName, bIsSimulate);
            this.Atlases.Clear();
            if (rLoadRequest.AllAssets == null)
                return;

            for (int i = 0; i < rLoadRequest.AllAssets.Length; i++)
            {
                UIAtlas rAtlas = rLoadRequest.AllAssets[i] as UIAtlas;
                if (rAtlas != null)
                {
                    this.Atlases.Add(rAtlas);
                }
            }
        }

        public IAsyncOperation<SpriteRequest> LoadSprite(string rSpriteName)
        {
            UIAtlas rSearchAtlas = this.SearchAtlas(rSpriteName);
            if (rSearchAtlas == null) return null;

            SpriteRequest rRequest = new SpriteRequest(rSearchAtlas.ABName, rSpriteName);
            if (rSearchAtlas.Mode == UIAtlasMode.Atlas)
            {
                return rRequest.Start(LoadSprite_AtlasMode_Async(rRequest));
            }
            else if (rSearchAtlas.Mode == UIAtlasMode.FullBG)
            {
                return rRequest.Start(LoadSprite_FullBGMode_Async(rRequest));
            }
            else if (rSearchAtlas.Mode == UIAtlasMode.Icons)
            {
                return rRequest.Start(LoadSprite_IconsMode_Async(rRequest));
            }
            return null;
        }

        public void UnloadSprite(string rSpriteName)
        {
            UIAtlas rSearchAtlas = this.SearchAtlas(rSpriteName);
            if (rSearchAtlas == null) return;

            string rABPAth = rSearchAtlas.ABName;
            if (rSearchAtlas.Mode == UIAtlasMode.FullBG)
                rABPAth = rSearchAtlas.ABName + "/" + rSpriteName.ToLower() + ".ab";

            ABLoader.Instance.UnloadAsset(rABPAth);
        }

        private IEnumerator LoadSprite_AtlasMode_Async(SpriteRequest rSpriteRequest)
        {
            bool bIsSumilate = ABPlatform.Instance.IsSumilateMode_GUI();
            var rLoadRequest = ABLoader.Instance.LoadAsset(rSpriteRequest.Path, rSpriteRequest.AssetName, bIsSumilate);
            yield return rLoadRequest;

            if (rLoadRequest.Result.Asset == null)
            {
                rSpriteRequest.SetResult(rSpriteRequest);
                yield break;
            }            
            if (!bIsSumilate)
            {
                rSpriteRequest.Sprite = rLoadRequest.Result.Asset as Sprite;
            }
            else
            {
                Texture2D rTex2D = rLoadRequest.Result.Asset as Texture2D;
                rSpriteRequest.Sprite = Sprite.Create(rTex2D, new Rect(0, 0, rTex2D.width, rTex2D.height), new Vector2(0.5f, 0.5f));
            }
            rSpriteRequest.SetResult(rSpriteRequest);
            ABLoader.Instance.UnloadAsset(rSpriteRequest.Path);
        }

        private IEnumerator LoadSprite_FullBGMode_Async(SpriteRequest rSpriteRequest)
        {
            string rABPath = rSpriteRequest.Path + "/" + rSpriteRequest.AssetName.ToLower() + ".ab";

            var rLoadRequest = ABLoader.Instance.LoadAsset(rABPath, rSpriteRequest.AssetName, ABPlatform.Instance.IsSumilateMode_GUI());
            yield return rLoadRequest;

            if (rLoadRequest.Result == null || rLoadRequest.Result.Asset == null)
            {
                rSpriteRequest.SetResult(rSpriteRequest);
                yield break;
            }

            rSpriteRequest.Texture = rLoadRequest.Result.Asset as Texture2D;

            rSpriteRequest.SetResult(rSpriteRequest);
        }

        private IEnumerator LoadSprite_IconsMode_Async(SpriteRequest rSpriteRequest)
        {
            var rLoadRequest = ABLoader.Instance.LoadAsset(rSpriteRequest.Path, rSpriteRequest.AssetName, ABPlatform.Instance.IsSumilateMode_GUI());
            yield return rLoadRequest;

            if (rLoadRequest.Result == null || rLoadRequest.Result.Asset == null)
            {
                rSpriteRequest.SetResult(rSpriteRequest);
                yield break;
            }

            rSpriteRequest.Sprite = rLoadRequest.Result.Asset as Sprite;
            ABLoader.Instance.UnloadAsset(rSpriteRequest.Path);

            rSpriteRequest.SetResult(rSpriteRequest);
        }

        public UIAtlas SearchAtlas(string rSpriteName)
        {
            if (this.Atlases == null) return null;

            for (int i = 0; i < this.Atlases.Count; i++)
            {
                int nIndex = this.Atlases[i].Sprites.FindIndex((rItem)=> { return rItem.Equals(rSpriteName); });
                if (nIndex >= 0)
                    return this.Atlases[i];
            }
            return null;
        }
    }
}
