using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTwistTest : MonoBehaviour {
    public Material mMyMaterial;
    [Range(0,100)]
    public float mTwist = 1;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (mMyMaterial)
        {
            mMyMaterial.SetFloat("_Twist", mTwist);
            Graphics.Blit(source, destination, mMyMaterial);
        }
    }
}
