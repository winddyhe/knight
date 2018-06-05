using UnityEngine;
using System.Collections;

namespace Knight.Framework.Graphics
{
    [ExecuteInEditMode]
    public class RapidBlurEffect : MonoBehaviour
    {
        //指定Shader名称
        private string          ShaderName = "WindShaderLab/PostEffect/RapidBlurEffect";

        //着色器和材质实例
        public Shader           CurShader;
        private Material        CurMaterial;

        //几个用于调节参数的中间变量
        public static int       ChangeValue;
        public static float     ChangeValue2;
        public static int       ChangeValue3;

        //降采样次数
        [Range(0, 6), Tooltip("[降采样次数]向下采样的次数。此值越大,则采样间隔越大,需要处理的像素点越少,运行速度越快。")]
        public int              DownSampleNum = 2;
        //模糊扩散度
        [Range(0.0f, 20.0f), Tooltip("[模糊扩散度]进行高斯模糊时，相邻像素点的间隔。此值越大相邻像素间隔越远，图像越模糊。但过大的值会导致失真。")]
        public float            BlurSpreadSize = 3.0f;
        //迭代次数
        [Range(0, 8), Tooltip("[迭代次数]此值越大,则模糊操作的迭代次数越多，模糊效果越好，但消耗越大。")]
        public int              BlurIterations = 3;
        
        Material material
        {
            get
            {
                if (CurMaterial == null)
                {
                    CurMaterial = new Material(CurShader);
                    CurMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return CurMaterial;
            }
        }
        
        void Start()
        {
            ChangeValue = DownSampleNum;
            ChangeValue2 = BlurSpreadSize;
            ChangeValue3 = BlurIterations;

            CurShader = Shader.Find(ShaderName);
            
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }
        }
        
        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (CurShader != null)
            {
                float widthMod = 1.0f / (1.0f * (1 << DownSampleNum));
                material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod);
                sourceTexture.filterMode = FilterMode.Bilinear;
                int renderWidth = sourceTexture.width >> DownSampleNum;
                int renderHeight = sourceTexture.height >> DownSampleNum;
                
                RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                renderBuffer.filterMode = FilterMode.Bilinear;
                UnityEngine.Graphics.Blit(sourceTexture, renderBuffer, material, 0);

                for (int i = 0; i < BlurIterations; i++)
                {
                    float iterationOffs = (i * 1.0f);
                    material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod + iterationOffs);
                    
                    RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                    UnityEngine.Graphics.Blit(renderBuffer, tempBuffer, material, 1);
                    RenderTexture.ReleaseTemporary(renderBuffer);
                    renderBuffer = tempBuffer;

                    tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                    UnityEngine.Graphics.Blit(renderBuffer, tempBuffer, CurMaterial, 2);

                    RenderTexture.ReleaseTemporary(renderBuffer);
                    renderBuffer = tempBuffer;
                }

                UnityEngine.Graphics.Blit(renderBuffer, destTexture);
                RenderTexture.ReleaseTemporary(renderBuffer);
            }
            else
            {
                UnityEngine.Graphics.Blit(sourceTexture, destTexture);
            }
        }
        
        void OnValidate()
        {
            ChangeValue = DownSampleNum;
            ChangeValue2 = BlurSpreadSize;
            ChangeValue3 = BlurIterations;
        }
        
        void Update()
        {
            if (Application.isPlaying)
            {
                DownSampleNum = ChangeValue;
                BlurSpreadSize = ChangeValue2;
                BlurIterations = ChangeValue3;
            }
#if UNITY_EDITOR
            if (Application.isPlaying != true)
            {
                CurShader = Shader.Find(ShaderName);
            }
#endif
        }
        
        void OnDisable()
        {
            if (CurMaterial)
            {
                DestroyImmediate(CurMaterial);
            }
        }
    }
}