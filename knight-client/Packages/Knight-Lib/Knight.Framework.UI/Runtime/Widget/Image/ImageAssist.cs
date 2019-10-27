using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Image))]
    [ExecuteInEditMode]
    public class ImageAssist : MonoBehaviour
    {
        [Range(0, 3)]
        public float    AdjustValue = 0.2f;
        public int      BlendSrc = 5;
        public int      BlendDst = 10;

        public Material AdjustMat;
        public Image    Image;

        private void Awake()
        {
            this.Image = this.GetComponent<Image>();

            if (this.AdjustMat != null)
                GameObject.DestroyImmediate(this.AdjustMat);
            this.AdjustMat = new Material(Shader.Find("ArclightRP/UI/UIAlphaAdjust"));
            this.Image.material = this.AdjustMat;

            this.UpdateParams();
        }

        private void OnDestroy()
        {
            this.Image.material = null;
            if (this.AdjustMat != null)
                GameObject.DestroyImmediate(this.AdjustMat);
            this.AdjustMat = null;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.UpdateParams();
        }
#endif

        public void UpdateParams()
        {
            if (this.AdjustMat != null)
            {
                this.AdjustMat.SetFloat("_Alpha", this.AdjustValue);
                this.AdjustMat.SetFloat("_SrcBlend", this.BlendSrc);
                this.AdjustMat.SetFloat("_DesBlend", this.BlendDst);
            }
        }
    }
}