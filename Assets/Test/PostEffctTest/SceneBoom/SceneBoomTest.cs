using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneBoomTest : MonoBehaviour {
    public Material mMyMaterial;
    [Range(0,10)]
    public float mSatCount=1;
    [Range(0,10)]
    public float mScaleX=0.1f;
    [Range(0,10)]
    public float mScaleY = 0.1f;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial)
        {
            mMyMaterial.SetFloat("_SatCount", mSatCount);
            mMyMaterial.SetFloat("_ScaleX", mScaleX);
            mMyMaterial.SetFloat("_ScaleY", mScaleY);
            Graphics.Blit(source, destination, mMyMaterial);
        }
    }
}
