using System;
using System.Collections.Generic;
using Core;
using System.Collections;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageReplace : MonoBehaviour
    {
        [SerializeField]
        private string              SpriteName;
        public  Image               Image;

        private CoroutineHandler    mLoadHandler;

        private void Awake()
        {
            this.Image = this.gameObject.ReceiveComponent<Image>();
            this.SpriteName = this.Image.sprite.name;
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
            
            this.SpriteName = rSpriteName;
            if (string.IsNullOrEmpty(rSpriteName))
            {
                this.Image.sprite = null;
                return;
            }

            CoroutineManager.Instance.Stop(mLoadHandler);
            mLoadHandler = CoroutineManager.Instance.StartHandler(LoadSprite_Async());
        }

        public void UnloadSprite()
        {
            UIAtlasManager.Instance.UnloadSprite(this.SpriteName);
        }

        private IEnumerator LoadSprite_Async()
        {
            if (string.IsNullOrEmpty(this.SpriteName))
            {
                this.Image.sprite = null;
                yield break;
            }

            var rRequest = UIAtlasManager.Instance.LoadSprite(this.SpriteName);
            yield return rRequest;

            if (rRequest == null || rRequest.Result == null || rRequest.Result.Sprite == null)
            {
                //Debug.LogErrorFormat("not find sprite: {0}", this.SpriteName);
                this.Image.sprite = null;
                yield break;
            }
            this.Image.sprite = rRequest.Result.Sprite;
        }
    }
}
