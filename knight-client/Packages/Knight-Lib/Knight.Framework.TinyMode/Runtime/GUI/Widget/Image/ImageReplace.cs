//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using Knight.Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Knight.Framework.TinyMode.UI  
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageReplace : MonoBehaviour
    {
        [SerializeField][ReadOnly]
        private string                  mSpriteName;
        public  Image                   Image;
        public  bool                    IsSetNativeSize;
        public  bool                    IsPeserveAspect;

        private void Awake()
        {
            this.Image = this.gameObject.ReceiveComponent<Image>();
            if (this.Image && this.Image.sprite)
            {
                this.mSpriteName = this.Image.sprite.name;
            }
            this.Image.sprite = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
        }

        private void OnDestroy()
        {
            this.Stop();
        }

        public void Stop()
        {
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
                this.Image.sprite = UIAtlasManager.Instance.LoadSprite(this.mSpriteName);
                if (this.IsSetNativeSize)
                {
                    this.Image.SetNativeSize();
                }
                this.Image.preserveAspect = this.IsPeserveAspect;
            }
        }
    }
}
