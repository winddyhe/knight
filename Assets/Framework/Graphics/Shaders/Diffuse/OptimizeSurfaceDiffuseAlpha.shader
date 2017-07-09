Shader "Optimize/OptimizeSurfaceDiffuseAlpha" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("双面",float)=1
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc("BlendSrc",float)=2
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendDes("BlendDes",float)=2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull[_Cull]
		Blend[_BlendSrc][_BlendDes]

		CGPROGRAM
		#pragma surface SimpleDiffuse Standard fullforwardshadows
		#pragma target 3.0
		#include "DiffuseCG.cginc"
		
		ENDCG
	}
	FallBack "OptimizeSurfaceDiffuse"
}
