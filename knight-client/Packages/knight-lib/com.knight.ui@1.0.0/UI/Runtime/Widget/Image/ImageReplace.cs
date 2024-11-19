using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Knight.Core;
using Cysharp.Threading.Tasks;

namespace Knight.Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageReplace : MonoBehaviour
    {
        public enum ImageReplaceType
        {
            Sprite,
            Icon,
        }

        public ImageReplaceType ImageType;

        [SerializeField, ReadOnly]
        private string mSpriteName;
        public string PrefabName;

        public Image Image;
        public bool IsNativeSize;
        public bool IsUseMultiLanguage;

        private Dictionary<string, AssetLoaderRequest<Sprite>> mLoaderRequestDict;
        private List<string> mRemovedRequestList;

        private void Awake()
        {
            this.mLoaderRequestDict = new Dictionary<string, AssetLoaderRequest<Sprite>>();
            this.mRemovedRequestList = new List<string>();
            this.Image = this.gameObject.ReceiveComponent<Image>();
            if (this.Image && this.Image.sprite)
            {
                this.mSpriteName = this.Image.sprite.name;
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
                UIAtlasManager.Instance.UnloadSprite(rPair.Value);
            }
            this.mLoaderRequestDict.Clear();
            this.mRemovedRequestList.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (this.Image && this.Image.sprite)
            {
                this.mSpriteName = this.Image.sprite.name;
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
                if (string.IsNullOrEmpty(this.mSpriteName))
                {
                    this.Image.sprite = null;
                    return;
                }
                this.Image.sprite = null;
                this.LoadSprite(this.ImageType, this.mSpriteName).WrapErrors();
            }
        }

        private async UniTask LoadSprite(ImageReplaceType rImageType, string rSpriteName)
        {
            AssetLoaderRequest<Sprite> rLoaderRequest = null;
            if (rImageType == ImageReplaceType.Sprite)
            {
                if (this.IsUseMultiLanguage)
                {
                    LogManager.LogError("Sprite cannot support multilanguage, please change icon type.");
                    return;
                }
                rLoaderRequest = UIAtlasManager.Instance.LoadSprite(rSpriteName, this.PrefabName);
            }
            else if (rImageType == ImageReplaceType.Icon)
            {
                if (!this.IsUseMultiLanguage)
                {
                    rLoaderRequest = UIAtlasManager.Instance.LoadIcon(rSpriteName);
                }
                else
                {
                    rLoaderRequest = UIAtlasManager.Instance.LoadMultiIcon(rSpriteName);
                }
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
                this.Image.sprite = null;
                return;
            }
            this.Image.sprite = rLoaderRequest.Asset;
            if (this.Image.sprite != null && this.IsNativeSize)
            {
                this.Image.SetNativeSize();
            }
            this.mRemovedRequestList.Clear();
            foreach (var rPair in this.mLoaderRequestDict)
            {
                if (rPair.Key.Equals(rSpriteName))
                {
                    continue;
                }
                UIAtlasManager.Instance.UnloadSprite(rPair.Value);
                this.mRemovedRequestList.Add(rPair.Key);
            }
            for (int i = 0; i < this.mRemovedRequestList.Count; i++)
            {
                this.mLoaderRequestDict.Remove(this.mRemovedRequestList[i]);
            }
        }
    }
}
