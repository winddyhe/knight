using Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class RawImageReplace : MonoBehaviour
    {
        [SerializeField]
        private string              SpriteName;
        public  RawImage            RawImage;

        private CoroutineHandler    mLoadHandler;
        
        private void Awake()
        {
            this.RawImage = this.gameObject.ReceiveComponent<RawImage>();
            this.SpriteName = this.RawImage.texture.name;
        }

        private void OnDestroy()
        {
            CoroutineManager.Instance.Stop(mLoadHandler);
        }

        public void ReplaceSprite(string rSpriteName)
        {
            if (this.SpriteName == rSpriteName)
            {
                return;
            }

            this.UnloadSprite();

            this.SpriteName = rSpriteName;
            if (string.IsNullOrEmpty(rSpriteName))
            {
                this.RawImage.texture = null;
                return;
            }

            CoroutineManager.Instance.Stop(this.mLoadHandler);
            this.mLoadHandler = CoroutineManager.Instance.StartHandler(LoadSprite_Async());
        }

        public void UnloadSprite()
        {
            UIAtlasManager.Instance.UnloadSprite(this.SpriteName);
        }

        private IEnumerator LoadSprite_Async()
        {
            if (string.IsNullOrEmpty(this.SpriteName))
            {
                this.RawImage.texture = null;
                yield break;
            }

            var rRequest = UIAtlasManager.Instance.LoadSprite(this.SpriteName);
            yield return rRequest;
            
            if (rRequest == null || rRequest.Result == null || rRequest.Result.Texture == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.SpriteName);
                yield break;
            }
            this.RawImage.texture = rRequest.Result.Texture;
        }
    }
}
