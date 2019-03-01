using Knight.Core;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class RawImageReplace : MonoBehaviour
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
        public  RawImage                RawImage;

        private LoadRequest             mLoadRequest;
        
        private void Awake()
        {
            this.RawImage = this.gameObject.ReceiveComponent<RawImage>();
            this.mSpriteName = this.RawImage.texture.name;
            this.mLoadRequest = new LoadRequest(this.mSpriteName);
        }

        private void OnDestroy()
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
                this.UnloadSprite();
                this.mSpriteName = value;
                if (string.IsNullOrEmpty(value))
                {
                    this.RawImage.texture = null;
                    return;
                }
                if (CoroutineManager.Instance != null)
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
                this.RawImage.texture = null;
                rRequest.SetResult(rRequest);
                yield break;
            }

            var rLoadRequest = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
            yield return rLoadRequest;
            
            if (rLoadRequest == null || rLoadRequest.Result == null || rLoadRequest.Result.Texture == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.mSpriteName);
                rRequest.SetResult(rRequest);
                yield break;
            }
            this.RawImage.texture = rLoadRequest.Result.Texture;
            rRequest.SetResult(rRequest);
        }
    }
}
