Shader "Axiu/EffectsShader"
{
    Properties
    {
        //IsUIShader
        [HideInInspector]_IsUIShader("_IsUIShader",Int) = 0
        //ShaderGUIBlendMode
        [HideInInspector]_BlendTemp("BlendTemp",float) = 0
        [Header(RenderOpation)]
        _ColorMask("ColorMask",Int) = 15
        //深度写入,提出模式，深度测试，混合模式，是否开启剪裁
        [Enum(Off,0,On,1)]_ZWrite("ZWrite",Int) = 0
        [Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode",Int) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest",Int) = 4
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendModeSrc("BlendModeSrc",Int) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_BlendModeDst("BlendModeDst",Int) = 10
        [Toggle(_CLIP_ON)]_Clip_On("Clip_On",Int) = 0
        [Header(Properties)]
        [Space(10)]
        //是否打开panner（所有纹理）
        [Toggle(_PANNER_ON)]_Panner_On("Panner_On",int) = 0
        //Shader是否用在粒子系统上
        [Enum(Off,0,Panner,1,Dissolve,2)]_ParticleMode("ParticleMode",Int) = 0
        //主纹理
        [HDR]_MainColor("MainColor",Color) = (1,1,1,1)
        [Enum(Alpha,0,R,1)]_MainTexAlpha("MainTexAlpha",Int) = 0
        _MainTex ("MainTex", 2D) = "white" {}
        _MainTex_PannerSpeedU("MainTex_PannerSpeedU",float) = 0.1
        _MainTex_PannerSpeedV("MainTex_PannerSpeedV",float) = 0.1
        [Space(10)]
        //Mask
        [Toggle(_MASK_ON)]_Mask_ON("Mask_ON",int) = 0
        [Enum(Alpha,0,R,1)]_MaskTexAlpha("MaskTexAlpha",Int) = 1
        _MaskTex("MaskTex",2D) = "white"{}
        _MaskTex_PannerSpeedU("Mask_PannerSpeedU",float) = 0
        _MaskTex_PannerSpeedV("Mask_PannerSpeedV",float) = 0
        [Space(10)]
        //溶解
        [Enum(Off,0,Dissolve,1,DoubleDissolve,2)]_DissolveMode("DissolveMode",int) = 0
        _DissolveTex("DissolveTex",2D) = "white"{}
        _DissolveFactor("DissolveFactor",Range(0,1)) = 0.5
        _HardnessFactor("HardnessFactor",Range(0,1)) = 0.9
        _DissolveWidth("DissolveWidth",Range(0,1)) = 0.1
        [HDR]_WidthColor("WidthColor",Color) = (1,1,1,1)
        _DissolveTex_PannerSpeedU("DissolveTex_PannerSpeedU",float) = 0
        _DissolveTex_PannerSpeedV("DissolveTex_PannerSpeedV",float) = 0
        [Space(10)]
        //扰动
        [Toggle(_NOISE_ON)]_Noise_On("Noise_On",int) = 0
        _NoiseTex("NoiseTex",2D) = "white"{}
        _NoiseIntensity("NoiseIntensity",float) = 0.5
        _NoiseTex_PannerSpeedU("NoiseTex_PannerSpeedU",float) = 0
        _NoiseTex_PannerSpeedV("NoiseTex_PannerSpeedV",float) = 0
        [Spcae(10)]
        //自发光
        [Toggle(_EMISSION_ON)]_Emission_On("Emission_On",int) = 0
        [HDR]_EmissionColor("EmissionColor",Color) = (1,1,1,1)
        _EmissionTex("EmissionTex",2D) = "white" {}
        _EmissionTex_PannerSpeedU("EmissionTex_PannerSpeedU",float) = 0
        _EmissionTex_PannerSpeedV("EmissionTex_PannerSpeedV",float) = 0
        [Space(10)]
        //frensle和边缘虚化
        [Enum(DISABLE,0,ADD,1,ALPHA,2)]_FresnelMode("FresnelMode",int) = 0
        [HDR]_FresnelColor("FresnelColor",Color) = (1,1,1,1)
        _FresnelWidth("FresnelWidth",Range(0,1)) = 0.5
        //Stencil
        [IntRange]_Stencil("StencilRef",Range(0,255)) = 255
        [IntRange]_StencilReadMask("StencilReadMask",Range(0,255)) = 255
        [IntRange]_StencilWriteMask("StencilWriteMask",Range(0,255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp("StencilComp",float) = 8
        [Enum(UnityEngine.Rendering.StencilOp)]_StencilOp("StencilOp",float) = 0

    }
    SubShader
    {
        Stencil
        {
            Ref [_Stencil]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
            Comp [_StencilComp]
            Pass [_StencilOp]
        }

        Tags { "Queue" = "Transparent" }
        Cull  [_CullMode]
        Blend [_BlendModeSrc] [_BlendModeDst]
        ZTest [_ZTest]
        ZWrite [_ZWrite]
        ColorMask [_ColorMask]

        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _MASK_ON
            #pragma shader_feature _DISSOLVE
            #pragma shader_feature _DOUBLEDISSOLVE
            #pragma shader_feature _PANNER_ON
            #pragma shader_feature _NOISE_ON
            #pragma shader_feature _EMISSION_ON
            #pragma shader_feature _CLIP_ON
            #pragma shader_feature _FRESNEL_ADD 
            #pragma shader_feature _FRESNEL_ALPHA 
			#pragma shader_feature _ _TIMESTOP_EFFECT

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP
            #pragma multi_compile _ DOTS_INSTANCING_ON
            #pragma instancing_options renderinglayer
            #pragma multi_compile_instancing

			#pragma	shader_feature_local _TIMESTOP_GRAY_DISABLE

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                fixed4 VColor : COLOR;
                float4 normal : NORMAL;
                fixed4 customData1:TEXCOORD1;
                fixed4 customData2:TEXCOORD2;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 uv1 : TEXCOORD0;
                fixed4 customData1:TEXCOORD1;
                fixed4 customData2:TEXCOORD2;
                float4 uv2 : TEXCOORD3;
                float4 uv3 : TEXCOORD4;
                float4 worldPos:TEXCOORD5;
                float4 normal  :TEXCOORD6;
                fixed4 VColor:COLOR;
            };
            
            int _ParticleModeTemp01,_ParticleModeTemp02;

            CBUFFER_START(UnityPerMaterial)
            int _IsUIShader;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _DissolveTex_ST;
            float4 _NoiseTex_ST;
            float4 _EmissionTex_ST;
            int _MainTexAlpha;
            fixed4 _MainColor;

            //ui
            float4 _ClipRect;
            //RenderOpation
            int _BlendMode;
            int _CullMode;
            int _ZWrite;
            int _Clip_On;
            int _ParticleMode;
            int _DissolveMode;
            int _FresnelMode;
            //Properties
            //MainTex
            sampler2D _MainTex;
            fixed _MainTex_PannerSpeedU,_MainTex_PannerSpeedV;
            //MaskTex
            int _MaskTexAlpha;
            sampler2D _MaskTex;
            fixed _MaskTex_PannerSpeedU,_MaskTex_PannerSpeedV;
            //DissolveTex
            sampler2D _DissolveTex;
            fixed _DissolveFactor;
            fixed _HardnessFactor;
            fixed _DissolveWidth;
            fixed4 _WidthColor;
            fixed _DissolveTex_PannerSpeedU,_DissolveTex_PannerSpeedV;
            //_NoiseTex
            sampler2D _NoiseTex;
            fixed _NoiseIntensity;
            fixed _NoiseTex_PannerSpeedU,_NoiseTex_PannerSpeedV;
            //EmissionTex
            fixed4 _EmissionColor;
            sampler2D _EmissionTex;
            fixed _EmissionTex_PannerSpeedU,_EmissionTex_PannerSpeedV;
            //_FresnelColor
            fixed4 _FresnelColor;
            fixed _FresnelWidth;
            half _TimeStopGraySaturation;
            CBUFFER_END

            //smoothstep函数去掉平滑部分
            inline float Smoothstep_Simple(fixed c,fixed minValue, fixed maxValue)
            {
                c = (c - minValue)/(maxValue - minValue);
                c = saturate(c);
                return c ;
            }
            //单层溶解函数
            inline float4 DissolveFunction(fixed4 c,fixed dissolveTex,fixed dissolve,fixed hardness)
            {
                hardness = clamp(hardness,0.00001,0.999999);
                dissolveTex += dissolve * (2 - hardness);
                dissolveTex = 2 - dissolveTex;
                dissolveTex= Smoothstep_Simple(dissolveTex,hardness,1);
                c.a *= dissolveTex;
                return c;
            }
            //双层溶解函数
            inline float4 DoubleDissolveFunction(fixed4 c,fixed dissolveTex,fixed dissolve,fixed hardness,fixed width,fixed4 WidthColor)
            {
                hardness = clamp(hardness,0.00001,999999);
                dissolve *= (1 + width);
                fixed hardnessFactor = 2 - hardness;
                fixed dissolve01 = dissolve * hardnessFactor + dissolveTex;
                dissolve01 = Smoothstep_Simple((2-dissolve01),hardness,1);
                fixed dissolve02 = (dissolve - width) * hardnessFactor  + dissolveTex;
                dissolve02 = Smoothstep_Simple((2-dissolve02),hardness,1);
                c.rgb = lerp(WidthColor,c.rgb,dissolve01);
                c.a *= dissolve02;
                return c;
            }


            v2f vert (a2v v)
            {
                v2f o ;
                UNITY_INITIALIZE_OUTPUT(v2f,o);

                o.customData1 = v.customData1;
                o.customData2 = v.customData2;
                //UIShaderUnity内部会传
                o.worldPos.xyz = lerp((mul(unity_ObjectToWorld, v.vertex).xyz) , v.vertex.xyz , _IsUIShader);
                // o.worldPos.xyz = mul(unity_ObjectToWorld, v.vertex).xyz ;
                // o.worldPos.xyz = v.vertex.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal.xyz = UnityObjectToWorldNormal(v.normal.xyz);

                o.uv1.xy = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1.zw = TRANSFORM_TEX(v.uv, _MaskTex);
                o.uv2.xy = TRANSFORM_TEX(v.uv, _DissolveTex);
                o.uv2.zw = TRANSFORM_TEX(v.uv, _NoiseTex);
                o.uv3.xy = TRANSFORM_TEX(v.uv, _EmissionTex);
                //是否打开panner
                #ifdef _PANNER_ON
                    {
                        o.uv1.xy += _Time.y * float2(_MainTex_PannerSpeedU,_MainTex_PannerSpeedV);
                        o.uv1.zw += _Time.y * float2(_MaskTex_PannerSpeedU,_MaskTex_PannerSpeedV);
                        o.uv2.xy += _Time.y * float2(_DissolveTex_PannerSpeedU,_DissolveTex_PannerSpeedV);
                        o.uv2.zw += _Time.y * float2(_NoiseTex_PannerSpeedU,_NoiseTex_PannerSpeedV);
                        o.uv3.xy += _Time.y * float2(_EmissionTex_PannerSpeedU,_EmissionTex_PannerSpeedV);
                    }
                #endif
                //custom1 4个值控制uv偏移,panner模式控制uv1和2，Dissolve模式只支持maintex的偏移
                o.uv1 += v.customData1 * _ParticleModeTemp01;
                o.uv2 += v.customData1 * _ParticleModeTemp01;
                o.uv1.xy += v.customData1.yz * _ParticleModeTemp02;


                o.VColor = v.VColor;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 finalColor;

                //是否打开扰动
                #ifdef _NOISE_ON
                    {
                        fixed4 noiseTex = tex2D(_NoiseTex , i.uv2.zw);
                        noiseTex = (noiseTex * 2 - 1) * _NoiseIntensity ;
                        i.uv1.xy += noiseTex;
                        i.uv1.zw += noiseTex;
                        i.uv2.xy += noiseTex;
                        i.uv2.zw += noiseTex;
                        i.uv3.xy += noiseTex;
                    }
                #endif
                //MainTex
                float2 uv_MainTex = i.uv1.xy;
                fixed4 mainTex = tex2D(_MainTex, uv_MainTex ) ;
                mainTex.a = lerp(mainTex.a,mainTex.r,_MainTexAlpha);
                mainTex = mainTex * _MainColor * i.VColor;
                finalColor = mainTex;

                //是否打开自发光
                #ifdef _EMISSION_ON
                    {
                        fixed4 emissionTex = tex2D(_EmissionTex,i.uv3.xy) * _EmissionColor;
                        finalColor.rgb += (emissionTex.rgb * emissionTex.a);
                    }
                #endif

                // 是否打开溶解
                #ifdef _DISSOLVE
                    {
                        float2 uv_DissolveTex = i.uv2.xy;
                        fixed dissolveTex = tex2D(_DissolveTex,uv_DissolveTex);
                        _DissolveFactor = lerp(_DissolveFactor,i.customData1.x,_ParticleModeTemp02);
                        finalColor = DissolveFunction(finalColor,dissolveTex,_DissolveFactor,_HardnessFactor);
                    }
                #endif
                #ifdef _DOUBLEDISSOLVE
                    {
                        float2 uv_DissolveTex = i.uv2.xy;
                        fixed dissolveTex = tex2D(_DissolveTex,uv_DissolveTex);
                        _WidthColor = lerp(_WidthColor,i.customData2,_ParticleModeTemp02);
                        _DissolveFactor = lerp(_DissolveFactor,i.customData1.x,_ParticleModeTemp02);
                        finalColor = DoubleDissolveFunction(finalColor,dissolveTex,_DissolveFactor,_HardnessFactor,_DissolveWidth,_WidthColor);
                    }
                #endif
                
                // 是否打开mask
                #ifdef _MASK_ON
                    {
                        float2 uv_MaskTex = i.uv1.zw;
                        fixed4 maskTex  = tex2D(_MaskTex,uv_MaskTex);
                        fixed maskChannel = lerp(maskTex.a,maskTex.r,_MaskTexAlpha);
                        finalColor.a *= maskChannel;
                    }
                #endif

                //是否启用clip
                #ifdef _CLIP_ON
                    {
                        clip(finalColor.a -0.5);
                    }
                #endif

                //Fresnel和边缘虚化
                #ifdef _FRESNEL_ADD
                    {
                        fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                        float3 normal = normalize(i.normal.xyz);
                        fixed dotVN = dot(viewDir , normal);
                        dotVN = 1 - saturate(dotVN );
                        dotVN = Smoothstep_Simple(dotVN,(1 - _FresnelWidth),1);
                        fixed4 fresnelColor = _FresnelColor * dotVN;
                        finalColor.rgb += fresnelColor.rgb;
                        finalColor.a  = finalColor.a * (lerp(dotVN,1,_FresnelColor.a));
                    }
                #endif
                #ifdef _FRESNEL_ALPHA
                    {
                        fixed3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos.xyz);
                        float3 normal = normalize(i.normal.xyz);
                        fixed dotVN = dot(viewDir ,normal);
                        dotVN = 1 - saturate(dotVN );
                        dotVN = Smoothstep_Simple(dotVN,(1 - _FresnelWidth),1);
                        finalColor.a *= (1 - dotVN);
                    }
                #endif

                //rect mask
                #ifdef UNITY_UI_CLIP_RECT
                    finalColor.a *= UnityGet2DClipping(i.worldPos.xy, _ClipRect);
                #endif
                //sprite mask
                #ifdef UNITY_UI_ALPHACLIP
                    clip (finalColor.a - 0.001);
                #endif
                // 时停灰化
	            #if defined(_TIMESTOP_EFFECT) && !defined(_TIMESTOP_GRAY_DISABLE)
		            finalColor.rgb = lerp(finalColor.rgb,
	                    dot(half3(0.299, 0.587, 0.114), finalColor.rgb) * half3(0.5, 0.5, 1),
	                        _TimeStopGraySaturation);
	            #endif

                return finalColor;
            }

            ENDHLSL
        }
    }
    CustomEditor "CustomEffectShaderGUI"
}
