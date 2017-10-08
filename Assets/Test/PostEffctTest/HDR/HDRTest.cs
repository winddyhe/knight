using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDRTest : MonoBehaviour {

    public Material mMyMaterial;
    public Color mColor = Color.white;
    [Range(0,4)]
    public float mBrightPow = 1;
    [Range(0,4)]
    public float mGamaPow = 1;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial != null)
        {
            mMyMaterial.SetColor("_Color", mColor);
            mMyMaterial.SetFloat("_BrightPow", mBrightPow);
            mMyMaterial.SetFloat("_GamaPow", mGamaPow);
            Graphics.Blit(source, destination, mMyMaterial);
        }
                
    }
}
