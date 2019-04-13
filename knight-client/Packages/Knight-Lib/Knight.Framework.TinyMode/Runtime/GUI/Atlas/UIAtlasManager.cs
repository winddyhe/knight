//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System;
using System.Collections.Generic;
using Knight.Core;
using System.Collections;
using System.Threading.Tasks;
using UnityFx.Async;
using Knight.Framework;
using UnityEngine;

namespace Knight.Framework.TinyMode.UI
{
    public class UIAtlasManager : TSingleton<UIAtlasManager>
    {
        public class SpriteRequest : AsyncRequest<SpriteRequest>
        {
            public Sprite   Sprite;
            public Texture  Texture;

            public UIAtlas  Atlas;
            public string   AssetName;

            public SpriteRequest(UIAtlas rAtlas, string rAssetName)
            {
                this.Atlas = rAtlas;
                this.AssetName = rAssetName;
            }
        }

        public List<UIAtlas> Atlases;

        private UIAtlasManager()
        {
            this.Atlases = new List<UIAtlas>();
        }

        public void Load(string rAtlasConfigABName)
        {
            var rLoadRequest = AssetLoader.Instance.LoadAllAssets(rAtlasConfigABName, typeof(UIAtlas));
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

        public Sprite LoadSprite(string rSpriteName)
        {
            if (this.Atlases == null) return null;

            for (int i = 0; i < this.Atlases.Count; i++)
            {
                int nIndex = this.Atlases[i].Sprites.FindIndex((rItem) => { return rItem.name.Equals(rSpriteName); });
                if (nIndex >= 0)
                {
                    return this.Atlases[i].Sprites[nIndex];
                }
            }
            return null;
        }

        public Texture2D LoadTexture(string rTexName)
        {
            if (this.Atlases == null) return null;

            for (int i = 0; i < this.Atlases.Count; i++)
            {
                int nIndex = this.Atlases[i].Textures.FindIndex((rItem) => { return rItem.name.Equals(rTexName); });
                if (nIndex >= 0)
                {
                    return this.Atlases[i].Textures[nIndex];
                }
            }
            return null;
        }
    }
}
