using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomEffectShaderGUI : ShaderGUI
{
    //渲染设置等
    #region 
    bool showRenderingOpation = true;
    MaterialProperty cullMode;
    bool isClipEnable;
    MaterialProperty blendTempProp, srcBlendProp, dstBlendProp;
    enum BlendMode
    {
        AlphaBlend, Add, Opaque
    }
    string[] blendModeNames = System.Enum.GetNames(typeof(BlendMode));

    enum ZTest
    {
        Default, Always
    }
    ZTest zTestEnum = ZTest.Default;
    MaterialProperty zTestProp;
    MaterialProperty particleModeProp;
    #endregion  //渲染设置等
    bool isPannerEnable;
    //MainTex
    #region 
    MaterialProperty mainTex, mainColor, mainTexAlphaChannel;
    MaterialProperty mainTexPannerX, mainTexPannerY;
    #endregion  //MainTex
    //Mask
    bool isMaskEnable;
    MaterialProperty maskTex, maskTexAlphaChannel, maskTexPannerX, maskTexPannerY;
    //溶解
    #region
    MaterialProperty dissolveMode, dissolveTex, dissolveFactor, dissolveHardness;
    MaterialProperty dissolveWidth, dissolveOutlineColor, dissolvePannerU, dissolvePannerV;
    int dissolveModeChoose;
    bool once = true;
    float cur = 0;
    #endregion //溶解
    //扰动
    bool isNoiseEnable;
    MaterialProperty noiseTex, noiseIntensity, noisePannerU, noisePannerV;
    //自发光
    bool isEmissionEnable;
    MaterialProperty emissionColor, emissionTex, emissionPannerU, emissionPannerV;
    //fresnel
    MaterialProperty fresnelColor, fresnelWidth, fresnelMode;
    int fresnelModeChoose;
    //Stencil
    MaterialProperty stencilRef, stencilReadMask, stencilWriteMask, stencilComp, stencilOp;
    bool isUIShader = false;
    //石化效果
    bool isPetrifactionEnable;
    MaterialProperty StatueTex, StatueDegree, Brightness, Tint,NormalTex, NormalScale, ShadowContrast, WorldShadowLightDir, ShadowColor;


    MaterialProperty isUIShaderON;


    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        GUILayout.Button("特效通用Shader");

        //获取材质球
        Material targetMat = materialEditor.target as Material;

        //折叠收起渲染设置列表
        showRenderingOpation = EditorGUILayout.Foldout(showRenderingOpation, "渲染设置");
        if (showRenderingOpation)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            isUIShaderON = FindProperty("_IsUIShader", props);
            isUIShader = isUIShaderON.floatValue == 1 ? true : false;

            isUIShader = EditorGUILayout.Toggle("是否为UIShader", isUIShader);
            if (isUIShader)
            {
                // targetMat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Always);
                targetMat.SetInt("_IsUIShader", 1);
            }
            else
            {
                // targetMat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Disabled);
                targetMat.SetInt("_IsUIShader", 0);
            }


            //更改混合模式
            #region 
            srcBlendProp = FindProperty("_BlendModeSrc", props);
            dstBlendProp = FindProperty("_BlendModeDst", props);
            blendTempProp = FindProperty("_BlendTemp", props);
            blendTempProp.floatValue = EditorGUILayout.Popup("混合模式(BlendMode)", (int)blendTempProp.floatValue, blendModeNames);
            //更改混合模式的时候设置一次Queue，之后可以手动更改
            if (blendTempProp.floatValue != cur)
            {
                cur = blendTempProp.floatValue;
                once = true;
            }
            if (once)
            {
                switch (blendTempProp.floatValue)
                {
                    case 0:
                        srcBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                        dstBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha;
                        //targetMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        targetMat.SetInt("_ZWrite", 0);
                        targetMat.SetOverrideTag("RenderType", "Transparent");
                        break;
                    case 1:
                        srcBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.SrcAlpha;
                        dstBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                        //targetMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        targetMat.SetInt("_ZWrite", 0);
                        targetMat.SetOverrideTag("RenderType", "Transparent");
                        break;
                    case 2:
                        srcBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.One;
                        dstBlendProp.floatValue = (float)UnityEngine.Rendering.BlendMode.Zero;
                        // targetMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                        targetMat.SetInt("_ZWrite", 1);
                        break;
                }
                once = false;
            }
            #endregion
            //深度测试
            #region
            zTestProp = FindProperty("_ZTest", props);
            switch (zTestProp.floatValue)
            {
                case 4:
                    zTestEnum = ZTest.Default;
                    break;
                case 8:
                    zTestEnum = ZTest.Always;
                    break;
            }
            zTestEnum = (ZTest)EditorGUILayout.EnumPopup("深度测试(ZTest)", zTestEnum);
            if (zTestEnum == ZTest.Default)
            {
                targetMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.LessEqual);
            }
            else
            {
                targetMat.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }
            #endregion

            //剔除模式CullMode
            cullMode = FindProperty("_CullMode", props);
            materialEditor.ShaderProperty(cullMode, "剔除模式（CullMode）");

            //是否打开clip
            #region 
            isPannerEnable = targetMat.IsKeywordEnabled("_CLIP_ON");
            isClipEnable = targetMat.IsKeywordEnabled("_CLIP_ON");
            isClipEnable = EditorGUILayout.Toggle("启用剪裁(Clip)", isClipEnable);
            if (isClipEnable)
            {
                targetMat.EnableKeyword("_CLIP_ON");
            }
            else
            {
                targetMat.DisableKeyword("_CLIP_ON");
            }
            #endregion

            //Stencil 设置
            #region
            stencilRef = FindProperty("_Stencil", props);
            materialEditor.ShaderProperty(stencilRef, "Stencil Ref");

            stencilReadMask = FindProperty("_StencilReadMask", props);
            materialEditor.ShaderProperty(stencilReadMask, "Stencil ReadMask");

            stencilWriteMask = FindProperty("_StencilWriteMask", props);
            materialEditor.ShaderProperty(stencilWriteMask, "Stencil WriteMask");

            stencilComp = FindProperty("_StencilComp", props);
            materialEditor.ShaderProperty(stencilComp, "Stencil Comp");

            stencilOp = FindProperty("_StencilOp", props);
            materialEditor.ShaderProperty(stencilOp, "Stencil Op");

            #endregion
            EditorGUILayout.EndVertical();
        }//折叠框括号


        //纹理长宽一致
        // materialEditor.SetDefaultGUIWidths();
        // base.OnGUI(materialEditor, props);
        EditorGUILayout.LabelField("属性", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        //粒子系统CustomData选择panner还是dissolve
        #region 
        particleModeProp = FindProperty("_ParticleMode", props);
        materialEditor.ShaderProperty(particleModeProp, "粒子系统CustomData模式");
        switch (particleModeProp.floatValue)
        {
            case 0:
                targetMat.SetInt("_ParticleModeTemp01", 0); targetMat.SetInt("_ParticleModeTemp02", 0);
                break;
            case 1:
                targetMat.SetInt("_ParticleModeTemp01", 1); targetMat.SetInt("_ParticleModeTemp02", 0);
                EditorGUILayout.HelpBox("CustomData1和2的xyzw分别对应主纹理、蒙版、溶解贴图、扰动贴图的uvPanner", MessageType.None);
                break;
            case 2:
                targetMat.SetInt("_ParticleModeTemp01", 0); targetMat.SetInt("_ParticleModeTemp02", 1);
                EditorGUILayout.HelpBox("Custom1.x的对应溶解值取值范围0~1,Y和Z控制主纹理的偏移，Custom2需要选择Color模式，对应亮边颜色", MessageType.None);
                break;
        }
        #endregion

        //是否启用panner
        #region
        isPannerEnable = targetMat.IsKeywordEnabled("_PANNER_ON");
        isPannerEnable = EditorGUILayout.Toggle("启用UV流动（所有纹理）", isPannerEnable);
        if (isPannerEnable)
        {
            targetMat.EnableKeyword("_PANNER_ON");
        }
        else
        {
            targetMat.DisableKeyword("_PANNER_ON");
        }

        EditorGUILayout.EndVertical();
        #endregion

        //主纹理颜色
        #region
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        mainTexAlphaChannel = FindProperty("_MainTexAlpha", props);
        materialEditor.ShaderProperty(mainTexAlphaChannel, "主纹理透明通道选择");
        mainColor = FindProperty("_MainColor", props);
        mainTex = FindProperty("_MainTex", props);
        materialEditor.TexturePropertyWithHDRColor(new GUIContent("主纹理(MainTex)"), mainTex, mainColor, true);
        materialEditor.TextureScaleOffsetProperty(mainTex);
        if (isPannerEnable)
        {
            mainTexPannerX = FindProperty("_MainTex_PannerSpeedU", props);
            mainTexPannerY = FindProperty("_MainTex_PannerSpeedV", props);
            materialEditor.ShaderProperty(mainTexPannerX, "UV流动U方向");
            materialEditor.ShaderProperty(mainTexPannerY, "UV流动V方向");
        }
        EditorGUILayout.EndVertical();
        #endregion

        //mask
        #region
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        isMaskEnable = targetMat.IsKeywordEnabled("_MASK_ON");
        isMaskEnable = EditorGUILayout.Toggle("启用蒙版(Mask)", isMaskEnable);
        if (isMaskEnable)
        {
            targetMat.EnableKeyword("_MASK_ON");

            maskTexAlphaChannel = FindProperty("_MaskTexAlpha", props);
            materialEditor.ShaderProperty(maskTexAlphaChannel, "蒙版通道选择");
            maskTex = FindProperty("_MaskTex", props);
            materialEditor.TexturePropertySingleLine(new GUIContent("蒙版遮罩(Mask)"), maskTex);
            materialEditor.TextureScaleOffsetProperty(maskTex);
            if (isPannerEnable)
            {
                maskTexPannerX = FindProperty("_MaskTex_PannerSpeedU", props);
                maskTexPannerY = FindProperty("_MaskTex_PannerSpeedV", props);
                materialEditor.ShaderProperty(maskTexPannerX, "UV流动U方向");
                materialEditor.ShaderProperty(maskTexPannerY, "UV流动U方向");
            }

        }
        else
        {
            targetMat.DisableKeyword("_MASK_ON");
        }
        EditorGUILayout.EndVertical();
        #endregion

        //溶解
        #region 
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        dissolveMode = FindProperty("_DissolveMode", props);
        materialEditor.ShaderProperty(dissolveMode, "启用溶解");
        switch (dissolveMode.floatValue)
        {
            case 0:
                targetMat.DisableKeyword("_DISSOLVE"); targetMat.DisableKeyword("_DOUBLEDISSOLVE");
                dissolveModeChoose = 0;
                break;
            case 1:
                targetMat.EnableKeyword("_DISSOLVE"); targetMat.DisableKeyword("_DOUBLEDISSOLVE");
                dissolveModeChoose = 1;
                break;
            case 2:
                targetMat.EnableKeyword("_DOUBLEDISSOLVE"); targetMat.DisableKeyword("_DISSOLVE");
                dissolveModeChoose = 2;
                break;
        }

        if (dissolveModeChoose == 1 || dissolveModeChoose == 2)
        {
            dissolveTex = FindProperty("_DissolveTex", props);
            dissolveFactor = FindProperty("_DissolveFactor", props);
            dissolveHardness = FindProperty("_HardnessFactor", props);
            dissolveWidth = FindProperty("_DissolveWidth", props);
            dissolveOutlineColor = FindProperty("_WidthColor", props);
            materialEditor.TexturePropertySingleLine(new GUIContent("溶解贴图(取R通道)"), dissolveTex);
            materialEditor.TextureScaleOffsetProperty(dissolveTex);
            materialEditor.ShaderProperty(dissolveFactor, "溶解值");
            materialEditor.ShaderProperty(dissolveHardness, "溶解软硬度");
            if (dissolveModeChoose == 2)
            {
                //粒子系统控制双层溶解的亮边颜色
                if (particleModeProp.floatValue != 2)
                {
                    materialEditor.ShaderProperty(dissolveOutlineColor, "溶解亮边颜色");
                }
                materialEditor.ShaderProperty(dissolveWidth, "溶解亮边宽度");
            }
            if (isPannerEnable)
            {
                dissolvePannerU = FindProperty("_DissolveTex_PannerSpeedU", props);
                dissolvePannerV = FindProperty("_DissolveTex_PannerSpeedV", props);
                materialEditor.ShaderProperty(dissolvePannerU, "UV流动U方向");
                materialEditor.ShaderProperty(dissolvePannerV, "UV流动V方向");
            }
        }
        EditorGUILayout.EndVertical();
        #endregion 

        //Noise扰动
        #region 
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        isNoiseEnable = targetMat.IsKeywordEnabled("_NOISE_ON");
        isNoiseEnable = EditorGUILayout.Toggle("启用扰动贴图", isNoiseEnable);
        if (isNoiseEnable)
        {
            targetMat.EnableKeyword("_NOISE_ON");
            noiseTex = FindProperty("_NoiseTex", props);
            noiseIntensity = FindProperty("_NoiseIntensity", props);
            materialEditor.TexturePropertySingleLine(new GUIContent("扰动贴图(取R通道)"), noiseTex);
            materialEditor.TextureScaleOffsetProperty(noiseTex);
            materialEditor.ShaderProperty(noiseIntensity, "扰动强度");
            if (isPannerEnable)
            {
                noisePannerU = FindProperty("_NoiseTex_PannerSpeedU", props);
                noisePannerV = FindProperty("_NoiseTex_PannerSpeedV", props);
                materialEditor.ShaderProperty(noisePannerU, "UV流动U方向");
                materialEditor.ShaderProperty(noisePannerV, "UV流动V方向");
            }
        }
        else
        {
            targetMat.DisableKeyword("_NOISE_ON");
        }
        EditorGUILayout.EndVertical();
        #endregion

        //自发光
        #region
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        isEmissionEnable = targetMat.IsKeywordEnabled("_EMISSION_ON");
        isEmissionEnable = EditorGUILayout.Toggle("启用自发光", isEmissionEnable);
        if (isEmissionEnable)
        {
            targetMat.EnableKeyword("_EMISSION_ON");
            emissionColor = FindProperty("_EmissionColor", props);
            emissionTex = FindProperty("_EmissionTex", props);
            materialEditor.TexturePropertyWithHDRColor(new GUIContent("自发光(Emission)"), emissionTex, emissionColor, true);
            materialEditor.TextureScaleOffsetProperty(emissionTex);
            if (isPannerEnable)
            {
                emissionPannerU = FindProperty("_EmissionTex_PannerSpeedU", props);
                emissionPannerV = FindProperty("_EmissionTex_PannerSpeedV", props);
                materialEditor.ShaderProperty(emissionPannerU, "UV流动U方向");
                materialEditor.ShaderProperty(emissionPannerV, "UV流动V方向");
            }
        }
        else
        {
            targetMat.DisableKeyword("_EMISSION_ON");
        }
        EditorGUILayout.EndVertical();
        #endregion

        //Fresnel
        #region
        //UIShader不需要支持Fresnel
        if (!isUIShader)
        {

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            fresnelMode = FindProperty("_FresnelMode", props);
            materialEditor.ShaderProperty(fresnelMode, "启用菲尼尔或边缘虚化");
            switch (fresnelMode.floatValue)
            {
                case 0:
                    targetMat.DisableKeyword("_FRESNEL_ADD"); targetMat.DisableKeyword("_FRESNEL_ALPHA");
                    fresnelModeChoose = 0;
                    break;
                case 1:
                    targetMat.EnableKeyword("_FRESNEL_ADD"); targetMat.DisableKeyword("_FRESNEL_ALPHA");
                    fresnelModeChoose = 1;
                    break;
                case 2:
                    targetMat.EnableKeyword("_FRESNEL_ALPHA"); targetMat.DisableKeyword("_FRESNEL_ADD");
                    fresnelModeChoose = 2;
                    break;
            }

            if (fresnelModeChoose == 1 || fresnelModeChoose == 2)
            {
                fresnelColor = FindProperty("_FresnelColor", props);
                fresnelWidth = FindProperty("_FresnelWidth", props);
                if (fresnelModeChoose == 1)
                {
                    materialEditor.ShaderProperty(fresnelColor, "菲尼尔颜色");
                    materialEditor.ShaderProperty(fresnelWidth, "菲尼尔边缘宽度");
                }
                else
                {
                    materialEditor.ShaderProperty(fresnelWidth, "边缘虚化宽度");
                }
            }
            EditorGUILayout.EndVertical();

        }
        else
        {
            targetMat.DisableKeyword("_FRESNEL_ADD"); targetMat.DisableKeyword("_FRESNEL_ALPHA");
        }

        #endregion
        //石化 效果
        #region
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        isPetrifactionEnable = targetMat.IsKeywordEnabled("_PETRIFACTION_ON");
        isPetrifactionEnable = EditorGUILayout.Toggle("启用石化效果", isPetrifactionEnable);
        if (isPetrifactionEnable)
        {
            targetMat.EnableKeyword("_PETRIFACTION_ON");
            StatueTex = FindProperty("_StatueTex", props);
            NormalTex = FindProperty("_NormalTex", props);

            StatueDegree = FindProperty("_StatueDegree", props);
            Brightness = FindProperty("_Brightness", props);
            Tint = FindProperty("_Tint", props);
            NormalScale = FindProperty("_NormalScale", props);
            ShadowContrast = FindProperty("_ShadowContrast", props);
            WorldShadowLightDir = FindProperty("_WorldShadowLightDir", props);
            ShadowColor =  FindProperty("_ShadowColor", props);

          

            materialEditor.TexturePropertySingleLine(new GUIContent("石化贴图"), StatueTex);
            materialEditor.TextureScaleOffsetProperty(StatueTex);
           
           
            materialEditor.TexturePropertySingleLine(new GUIContent("法线纹理贴图"), NormalTex);
            materialEditor.TextureScaleOffsetProperty(NormalTex);
            materialEditor.ShaderProperty(StatueDegree, "石化明暗对比度");
            materialEditor.ShaderProperty(Brightness, "亮度");
            materialEditor.ShaderProperty(NormalScale, "法线强度");
            materialEditor.ShaderProperty(ShadowContrast, "阴影面积");
            materialEditor.ShaderProperty(WorldShadowLightDir, "阴影方向");


            materialEditor.ColorProperty(Tint, "石化颜色");
            materialEditor.ColorProperty(ShadowColor,"阴影颜色");
            




        }
        else
        {
            targetMat.DisableKeyword("_PETRIFACTION_ON");
        }
        EditorGUILayout.EndVertical();
        #endregion
        //材质层级
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        materialEditor.RenderQueueField();
        EditorGUILayout.EndVertical();



    }
}