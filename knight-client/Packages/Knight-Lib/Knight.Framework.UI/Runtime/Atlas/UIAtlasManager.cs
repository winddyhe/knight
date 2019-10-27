using System;
using System.Collections.Generic;
using Knight.Core;
using System.Collections;
using System.Threading.Tasks;
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

        public async Task Load(string rAtlasConfigABName)
        {
            bool bIsSimulate = AssetLoader.Instance.IsSumilateMode_GUI();

            var rLoadRequest = await AssetLoader.Instance.LoadAllAssetsAsync(rAtlasConfigABName, bIsSimulate);
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

        public SpriteRequest LoadSprite(string rSpriteName)
        {
            UIAtlas rSearchAtlas = this.SearchAtlas(rSpriteName);
            if (rSearchAtlas == null) return null;

            SpriteRequest rRequest = new SpriteRequest(rSearchAtlas.ABName.ToLower(), rSpriteName);
            if (rSearchAtlas.Mode == UIAtlasMode.Atlas)
            {
                return LoadSprite_AtlasMode_Sync(rRequest);
            }
            else if (rSearchAtlas.Mode == UIAtlasMode.FullBG)
            {
                return LoadSprite_FullBGMode_Sync(rRequest);
            }
            else if (rSearchAtlas.Mode == UIAtlasMode.Icons)
            {
                return LoadSprite_IconsMode_Sync(rRequest);
            }
            return null;
        }

        public void UnloadSprite(string rSpriteName)
        {
            UIAtlas rSearchAtlas = this.SearchAtlas(rSpriteName);
            if (rSearchAtlas == null) return;

            string rABPath = rSearchAtlas.ABName;
            if (rSearchAtlas.Mode == UIAtlasMode.FullBG)
            {
                rABPath = rSearchAtlas.ABName.ToLower() + "/" + rSpriteName.ToLower() + ".ab";
            }
            AssetLoader.Instance.UnloadAsset(rABPath);
        }

        private Vector4 mTempBorder = new Vector4(0, 0, 1, 1);
        private SpriteRequest LoadSprite_AtlasMode_Sync(SpriteRequest rSpriteRequest)
        {
            bool bIsSumilate = AssetLoader.Instance.IsSumilateMode_GUI();
            var rLoadRequest = AssetLoader.Instance.LoadAsset(rSpriteRequest.Path.ToLower(), rSpriteRequest.AssetName, bIsSumilate);

            if (rLoadRequest.Asset == null)
            {
                return null;
            }
            if (!bIsSumilate)
            {
                rSpriteRequest.Sprite = rLoadRequest.Asset as Sprite;
            }
            else
            {
                Texture2D rTex2D = rLoadRequest.Asset as Texture2D;
                var rRect = new Rect(0, 0, rTex2D.width, rTex2D.height);
                rSpriteRequest.Sprite = Sprite.Create(rTex2D, rRect, new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight, mTempBorder);
                rSpriteRequest.Sprite.name = rSpriteRequest.AssetName;
            }
            return rSpriteRequest;
        }

        private SpriteRequest LoadSprite_FullBGMode_Sync(SpriteRequest rSpriteRequest)
        {
            string rABPath = rSpriteRequest.Path.ToLower() + "/" + rSpriteRequest.AssetName.ToLower() + ".ab";

            var rLoadRequest = AssetLoader.Instance.LoadAsset(rABPath, rSpriteRequest.AssetName, AssetLoader.Instance.IsSumilateMode_GUI());
            if (rLoadRequest == null || rLoadRequest.Asset == null)
            {
                return null;
            }
            rSpriteRequest.Texture = rLoadRequest.Asset as Texture2D;
            return rSpriteRequest;
        }

        private SpriteRequest LoadSprite_IconsMode_Sync(SpriteRequest rSpriteRequest)
        {
            bool bIsSumilate = AssetLoader.Instance.IsSumilateMode_GUI();
            var rLoadRequest = AssetLoader.Instance.LoadAsset(rSpriteRequest.Path.ToLower(), rSpriteRequest.AssetName, bIsSumilate);
            if (rLoadRequest == null || rLoadRequest.Asset == null)
            {
                return null;
            }
            
            Texture2D rTex2D = rLoadRequest.Asset as Texture2D;
            rSpriteRequest.Sprite = Sprite.Create(rTex2D, new Rect(0, 0, rTex2D.width, rTex2D.height), new Vector2(0.5f, 0.5f), 100, 1, SpriteMeshType.Tight, mTempBorder);
            rSpriteRequest.Sprite.name = rSpriteRequest.AssetName;

            return rSpriteRequest;
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
