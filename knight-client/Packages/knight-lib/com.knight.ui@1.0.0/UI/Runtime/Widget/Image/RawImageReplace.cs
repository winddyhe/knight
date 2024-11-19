using Cysharp.Threading.Tasks;
using Knight.Core;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RawImage))]
    public class RawImageReplace : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private string mSpriteName;

        public RawImage RawImage;
        public bool IsNativeSize;
        public bool IsUseMultiLanguage;

        private Dictionary<string, AssetLoaderRequest<Texture2D>> mLoaderRequestDict;
        private List<string> mRemovedRequestList;

        private void Awake()
        {
            this.mLoaderRequestDict = new Dictionary<string, AssetLoaderRequest<Texture2D>>();
            this.mRemovedRequestList = new List<string>();
            this.RawImage = this.gameObject.ReceiveComponent<RawImage>();

            if (this.RawImage && this.RawImage.texture)
            {
                this.mSpriteName = this.RawImage.texture.name;
            }
            else if (this.RawImage && this.RawImage.texture)
            {
                this.mSpriteName = this.RawImage.texture.name;
            }
            else
            {
                this.mSpriteName = string.Empty;
            }
        }
        private void OnDestroy()
        {
            foreach (var rPair in this.mLoaderRequestDict)
            {
                UIAtlasManager.Instance.UnloadTexture(rPair.Value);
            }
            this.mLoaderRequestDict.Clear();
            this.mRemovedRequestList.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.RawImage && this.RawImage.texture)
            {
                this.mSpriteName = this.RawImage.texture.name;
            }
        }
#endif

        public string SpriteName
        {
            get
            {
                return this.mSpriteName;
            }
            set
            {
                if (this.mSpriteName == value)
                {
                    return;
                }
                this.mSpriteName = value;
                if (string.IsNullOrEmpty(value))
                {
                    this.RawImage.texture = null;
                    return;
                }
                this.LoadSprite(this.mSpriteName).WrapErrors();
            }
        }

        private async UniTask LoadSprite(string rSpriteName)
        {
            AssetLoaderRequest<Texture2D> rLoaderRequest = null;
            if (!this.IsUseMultiLanguage)
            {
                rLoaderRequest = UIAtlasManager.Instance.LoadTexture(this.mSpriteName);
            }
            else
            {
                rLoaderRequest = UIAtlasManager.Instance.LoadMultiTexture(this.mSpriteName);
            }
            if (rLoaderRequest == null)
            {
                return;
            }
            
            this.mLoaderRequestDict.Add(rSpriteName, rLoaderRequest);

            await rLoaderRequest.Task();
            if (rLoaderRequest == null || rLoaderRequest.Asset == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.SpriteName);
                this.RawImage.texture = null;
                return;
            }
            this.RawImage.texture = rLoaderRequest.Asset;
            if (this.RawImage.texture != null && this.IsNativeSize)
            {
                this.RawImage.SetNativeSize();
            }

            foreach (var rPair in this.mLoaderRequestDict)
            {
                if (rPair.Key.Equals(rSpriteName))
                {
                    continue;
                }
                UIAtlasManager.Instance.UnloadTexture(rPair.Value);
                this.mRemovedRequestList.Add(rPair.Key);
            }
            for (int i = 0; i < this.mRemovedRequestList.Count; i++)
            {
                this.mLoaderRequestDict.Remove(this.mRemovedRequestList[i]);
            }
        }
    }
}
