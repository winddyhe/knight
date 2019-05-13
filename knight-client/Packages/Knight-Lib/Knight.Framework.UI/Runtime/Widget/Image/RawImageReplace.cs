using Knight.Core;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(RawImage))]
    [ExecuteInEditMode]
    public class RawImageReplace : MonoBehaviour
    {
        [SerializeField][ReadOnly]
        private string                  mSpriteName;
        public  RawImage                RawImage;
        
        private void Awake()
        {
            this.RawImage = this.gameObject.ReceiveComponent<RawImage>();
            if (this.RawImage && this.RawImage.texture)
            {
                this.mSpriteName = this.RawImage.texture.name;
            }
        }

        private void OnDestroy()
        {
            this.UnloadSprite();
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
                this.LoadSprite(this.mSpriteName);
            }
        }

        public void UnloadSprite()
        {
            UIAtlasManager.Instance.UnloadSprite(this.mSpriteName);
        }

        private void LoadSprite(string rSpriteName)
        {
            if (string.IsNullOrEmpty(this.mSpriteName))
            {
                this.RawImage.texture = null;
                return;
            }

            var rLoadRequest = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
            if (rLoadRequest == null || rLoadRequest.Texture == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.mSpriteName);
                return;
            }
            this.RawImage.texture = rLoadRequest.Texture;
        }
    }
}
