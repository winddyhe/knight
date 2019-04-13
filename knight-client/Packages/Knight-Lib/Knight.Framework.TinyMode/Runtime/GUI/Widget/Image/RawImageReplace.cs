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
            this.mSpriteName = this.RawImage.texture.name;
            this.RawImage.texture = UIAtlasManager.Instance.LoadTexture(this.mSpriteName);
        }

        private void OnDestroy()
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
                    this.RawImage.texture = null;
                    return;
                }
                this.RawImage.texture = UIAtlasManager.Instance.LoadTexture(this.mSpriteName);
            }
        }
    }
}
