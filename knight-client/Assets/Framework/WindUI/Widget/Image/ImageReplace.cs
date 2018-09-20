using System;
using System.Collections.Generic;
using Knight.Core;
using System.Collections;
using NaughtyAttributes;
using System.Threading.Tasks;
using System.Threading;
using UnityFx.Async;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageReplace : MonoBehaviour
    {
        private class LoadRequest : AsyncRequest<LoadRequest>
        {
            private string              mSpriteName;

            public LoadRequest(string rSpriteName)
            {
                this.mSpriteName = rSpriteName;
            }
        }

        [SerializeField][ReadOnly]
        private string                  mSpriteName;
        public  Image                   Image;

        private LoadRequest             mLoadRequest;

        private void Awake()
        {
            this.Image = this.gameObject.ReceiveComponent<Image>();
            this.mSpriteName = this.Image?.sprite.name;
            this.mLoadRequest = new LoadRequest(this.mSpriteName);
        }

        private void OnDestroy()
        {
            this.Stop();
        }

        public void Stop()
        {
            if (this.mLoadRequest != null)
                this.mLoadRequest.Stop();
        }
        
        public string SpriteName
        {
            get { return this.mSpriteName; }
            set
            {
                if (this.mSpriteName == value)
                {
                    return;
                }
                this.mSpriteName = value;
                if (string.IsNullOrEmpty(value))
                {
                    this.Image.sprite = null;
                    return;
                }
                if (this.mLoadRequest != null)
                {
                    this.mLoadRequest.Stop();

                    this.mLoadRequest = new LoadRequest(this.mSpriteName);
                    this.mLoadRequest.Start(LoadSprite_Async(this.mLoadRequest));
                }
            }
        }

        public void UnloadSprite()
        {
            UIAtlasManager.Instance.UnloadSprite(this.mSpriteName);
        }

        private IEnumerator LoadSprite_Async(LoadRequest rRequest)
        {
            if (string.IsNullOrEmpty(this.mSpriteName))
            {
                this.Image.sprite = null;
                rRequest.SetResult(rRequest);
                yield break;
            }

            var rLoadRequest = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
            yield return rLoadRequest;

            if (rLoadRequest == null || rLoadRequest.Result == null || rLoadRequest.Result.Sprite == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.SpriteName);
                this.Image.sprite = null;
                rRequest.SetResult(rRequest);
                yield break;
            }
            this.Image.sprite = rLoadRequest.Result.Sprite;
        }
    }
}
