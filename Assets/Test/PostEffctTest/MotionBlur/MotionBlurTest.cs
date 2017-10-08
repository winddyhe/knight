using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBlurTest : MonoBehaviour {

    public Material mMyMaterial;
    [Range(0,1)]
    public float mBlurAmount = 0.5f;
    private RenderTexture mMyRenderTexture;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial)
        {
            if (mMyRenderTexture == null || mMyRenderTexture.width != Screen.width || mMyRenderTexture.height != Screen.height)
            {
                mMyRenderTexture = new RenderTexture(Screen.width, Screen.height,0);
                Graphics.Blit(source, mMyRenderTexture);
            }
            mMyMaterial.SetFloat("_BlurAmount", mBlurAmount);
            Graphics.Blit(source, mMyRenderTexture,mMyMaterial);
            Graphics.Blit(mMyRenderTexture, destination);
        }
    }
}
