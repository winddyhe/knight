using System;
using System.Collections.Generic;
using UnityEngine;
using Knight.Core;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    public class GrayGraphics : MonoBehaviour
    {
        public bool             IsGray = true;
        public Material         GrayMat;

        public List<Graphic>    WillGrayGraphics;

        private void Awake()
        {
            if (this.GrayMat != null)
                GameObject.DestroyImmediate(this.GrayMat);
            this.GrayMat = new Material(Shader.Find("UI/Default_Gray"));

            var rGraphics = this.GetComponents<Graphic>();
            for (int i = 0; i < rGraphics.Length; i++)
            {
                rGraphics[i].material = this.GrayMat;
                SetGraphicGray(rGraphics[i], this.IsGray);
            }

            for (int i = 0; i < this.WillGrayGraphics.Count; i++)
            {
                this.WillGrayGraphics[i].material = this.GrayMat;
                SetGraphicGray(this.WillGrayGraphics[i], this.IsGray);
            }
        }

        public void SetGray(bool bIsGray)
        {
            this.IsGray = bIsGray;

            var rGraphics = this.GetComponents<Graphic>();
            for (int i = 0; i < rGraphics.Length; i++)
            {
                rGraphics[i].material = this.GrayMat;
                SetGraphicGray(rGraphics[i], this.IsGray);
            }

            for (int i = 0; i < this.WillGrayGraphics.Count; i++)
            {
                this.WillGrayGraphics[i].material = this.GrayMat;
                SetGraphicGray(this.WillGrayGraphics[i], this.IsGray);
            }
        }

        private void OnDestroy()
        {
            var rGraphics = this.GetComponents<Graphic>();
            for (int i = 0; i < rGraphics.Length; i++)
            {
                rGraphics[i].material = null;
            }
            for (int i = 0; i < this.WillGrayGraphics.Count; i++)
            {
                this.WillGrayGraphics[i].material = null;
            }
            this.WillGrayGraphics.Clear();

            if (this.GrayMat != null)
                GameObject.DestroyImmediate(this.GrayMat);
            this.GrayMat = null;
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            var rGraphics = this.GetComponents<Graphic>();
            for (int i = 0; i < rGraphics.Length; i++)
            {
                SetGraphicGray(rGraphics[i], this.IsGray);
            }

            for (int i = 0; i < this.WillGrayGraphics.Count; i++)
            {
                this.WillGrayGraphics[i].material = this.GrayMat;
                SetGraphicGray(this.WillGrayGraphics[i], this.IsGray);
            }
        }
#endif

        private void SetGraphicGray(Graphic rGraphic, bool bIsGray)
        {
            if (bIsGray)
                rGraphic.material.EnableKeyword("UI_GRAY");
            else
                rGraphic.material.DisableKeyword("UI_GRAY");
        }
    }
}
