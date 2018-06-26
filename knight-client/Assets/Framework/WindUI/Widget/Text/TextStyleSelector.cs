using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI
{
    [RequireComponent(typeof(Text))]
    [ExecuteInEditMode]
    public class TextStyleSelector : MonoBehaviour
    {
        [HideInInspector]
        public Text Text;

        public int  StyleID;

        private void Awake()
        {
            if (this.Text == null)
                this.Text = this.GetComponent<Text>();
        }

        public void ApplyStyle(TextStyle rStyle)
        {
            StyleID = rStyle.ID;

            if (rStyle.Font)
                Text.font = rStyle.Font;

            Text.color = rStyle.Color;
            Text.fontSize = rStyle.FontSize;

            if (rStyle.UseGradient)
            {
                Gradient rGradient = Text.GetComponent<Gradient>();
                if (!rGradient)
                    rGradient = gameObject.AddComponent<Gradient>();

                rGradient.TopColor = rStyle.TopColor;
                rGradient.BottomColor = rStyle.BottomColor;
            }
            else
            {
                Gradient rGradient = Text.GetComponent<Gradient>();
                if (rGradient)
                    DestroyImmediate(rGradient);
            }

            if (rStyle.UseOutline)
            {
                Outline rOutline = Text.GetComponent<Outline>();
                if (!rOutline)
                    rOutline = gameObject.AddComponent<Outline>();

                rOutline.effectColor = rStyle.OutlineColor;
                rOutline.effectDistance = rStyle.OutlineDistance;
                rOutline.useGraphicAlpha = rStyle.ShadowGraphicAlpha;
            }
            else
            {
                Outline rOutline = Text.GetComponent<Outline>();
                if (rOutline)
                    DestroyImmediate(rOutline);
            }

            if (rStyle.UseShadow)
            {
                Shadow rShadow = Text.GetComponent<Shadow>();
                if (!rShadow)
                    rShadow = gameObject.AddComponent<Shadow>();

                rShadow.effectColor = rStyle.ShadowColor;
                rShadow.effectDistance = rStyle.ShadowDistance;
                rShadow.useGraphicAlpha = rStyle.ShadowGraphicAlpha;
            }
            else
            {
                Shadow rShadow = Text.GetComponent<Shadow>();
                if (rShadow)
                    DestroyImmediate(rShadow);
            }
        }
    }
}
