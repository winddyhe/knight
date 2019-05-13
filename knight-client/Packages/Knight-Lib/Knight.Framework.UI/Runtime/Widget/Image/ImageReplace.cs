using Knight.Core;
using NaughtyAttributes;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageReplace : MonoBehaviour
    {
        [SerializeField][ReadOnly]
        private string                  mSpriteName;
        public  Image                   Image;

        private void Awake()
        {
            this.Image = this.gameObject.ReceiveComponent<Image>();
            if (this.Image && this.Image.sprite)
            {
                this.mSpriteName = this.Image.sprite.name;
            }
        }

        private void OnDestroy()
        {
            UIAtlasManager.Instance.UnloadSprite(this.mSpriteName);
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
                this.Image.sprite = null;
                return;
            }
            var rLoadRequest = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
            if (rLoadRequest == null || rLoadRequest.Sprite == null)
            {
                Debug.LogErrorFormat("not find sprite: {0}", this.SpriteName);
                this.Image.sprite = null;
                return;
            }
            this.Image.sprite = rLoadRequest.Sprite;
        }
    }
}
