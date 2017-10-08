using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadialBlurTest : MonoBehaviour {

    public Material mMyMaterial;
    [Range(0,10)]
    public float mOffsetX;
    [Range(0,10)]
    public float mOffsetY;
    [Range(0,10)]
    public float mBlurSize = 1;
    public float mIterationNum = 3;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial)
        {
            mMyMaterial.SetFloat("_OffsetX", mOffsetX);
            mMyMaterial.SetFloat("_OffsetY", mOffsetY);
            mMyMaterial.SetFloat("_BlurSize", mBlurSize);
            mMyMaterial.SetFloat("_IterationNum", mIterationNum);
            Graphics.Blit(source, destination, mMyMaterial);
        }
    }
}
