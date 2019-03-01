Shader "WindShaderLab/Diffuse/DiffuseAlpha" 
{
	Properties 
	{
		_Color   ("Color", Color)	  = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" { }

		[Enum(UnityEngine.Rendering.CullMode)]  _Cull("双面",float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("BlendSrc",float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDes("BlendDes",float) = 1
	}

	SubShader 
	{
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		Fog  { Color(1.0, 1.0, 1.0, 1.0) }
		
		Cull  [_Cull]
		Blend [_BlendSrc][_BlendDes]

		CGPROGRAM
		#pragma surface SimpleDiffuse Standard fullforwardshadows
		#include "Diffuse.cginc"
		
		ENDCG
	}
}
