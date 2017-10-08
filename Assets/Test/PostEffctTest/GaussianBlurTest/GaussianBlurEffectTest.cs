using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GaussianBlurEffectTest : MonoBehaviour {

    public Material mMyMaterial;
    [Range(0,4)]
    public float mBlurSize=1;
    [Range(1,8)]
    public int mDownSampler = 1;
    [Range(0, 4)]
    public float mIterations = 3;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial != null)
        {
            int rScreenWidth = Screen.width / mDownSampler;
            int rScreendHeight = Screen.height / mDownSampler;
            var rTempTexture1 = RenderTexture.GetTemporary(rScreenWidth, rScreendHeight);
            Graphics.Blit(source, rTempTexture1);
            for (int i = 0; i < mIterations; i++)
            {
                mMyMaterial.SetFloat("_BlurSize", i * 1 * mBlurSize);
                var rTempTexture2 = RenderTexture.GetTemporary(rScreenWidth, rScreendHeight);
                Graphics.Blit(rTempTexture1, rTempTexture2, mMyMaterial, 0);
                RenderTexture.ReleaseTemporary(rTempTexture1);
                rTempTexture1 = RenderTexture.GetTemporary(rScreenWidth, rScreendHeight);
                Graphics.Blit(rTempTexture2, rTempTexture1, mMyMaterial, 1);
                RenderTexture.ReleaseTemporary(rTempTexture2);
            }
            Graphics.Blit(rTempTexture1, destination);
            RenderTexture.ReleaseTemporary(rTempTexture1);
        }
        else
            Graphics.Blit(source, destination);
    }
}
