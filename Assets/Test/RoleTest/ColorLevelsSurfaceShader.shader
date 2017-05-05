Shader "Custom/ColorLevelsSurfaceShader"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _inBlack("Input Black", Range(0, 255)) = 0
        _inGamma("Input Gamma", Range(0, 2)) = 1
        _inWhite("Input White", Range(0, 255)) = 255

        _outWhite("Output White", Range(0, 255)) = 255
        _outBlack("Output Black", Range(0, 255)) = 0
    }
    SubShader {
        Tags { "RenderType"="Transparent" }
        LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows
        // Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0
		
        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _inBlack;
        float _inGamma;
        float _inWhite;
        float _outWhite;
        float _outBlack;

        float GetPixelLevel(float inPixel)
        {
            return (pow(((inPixel * 255.0) - _inBlack) / (_inWhite - _inBlack), _inGamma) * (_outWhite - _outBlack) + _outBlack) / 255.0;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            c.rgb = fixed3(GetPixelLevel(c.r), GetPixelLevel(c.g), GetPixelLevel(c.b));
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}