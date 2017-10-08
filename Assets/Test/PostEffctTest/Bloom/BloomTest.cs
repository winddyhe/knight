using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomTest : MonoBehaviour {

    public Material mMyMaterial;
    [Range(0,10)]
    public float mLuminanceThreshold=1;
    [Range(0,4)]
    public int mBlurSize=1;
    [Range(1,4)]
    public int mIteration=3;
    [Range(1,8)]
    public int mDownSampler=1;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        int rScreenWidth    = Screen.width / mDownSampler;
        int rScreenHeight   = Screen.height / mDownSampler;
        var rTempTexture1   = RenderTexture.GetTemporary(rScreenWidth, rScreenHeight);
        mMyMaterial.SetFloat("_LuminanceThreshold", mLuminanceThreshold);
        Graphics.Blit(source, rTempTexture1, mMyMaterial, 0);
        mMyMaterial.SetTexture("_Bloom", rTempTexture1);
        for (int i = 0; i < mIteration; i++)
        {
            mMyMaterial.SetFloat("_BlurSize",  1+i * mBlurSize);
            var rTempTexture2 = RenderTexture.GetTemporary(rScreenWidth, rScreenHeight);
            Graphics.Blit(rTempTexture1, rTempTexture2, mMyMaterial, 1);
            RenderTexture.ReleaseTemporary(rTempTexture1);
            rTempTexture1 = RenderTexture.GetTemporary(rScreenWidth, rScreenHeight);
            Graphics.Blit(rTempTexture2, rTempTexture1, mMyMaterial, 2);
            RenderTexture.ReleaseTemporary(rTempTexture2);
        }
        Graphics.Blit(rTempTexture1, destination, mMyMaterial, 3);
        RenderTexture.ReleaseTemporary(rTempTexture1);
    }
}
