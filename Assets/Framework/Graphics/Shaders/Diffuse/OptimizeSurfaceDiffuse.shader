Shader "Optimize/OptimizeSurfaceDiffuse" {
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("双面",float)=1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull[_Cull]
		
		CGPROGRAM
		#include "DiffuseCG.cginc"
		#pragma surface SimpleDiffuse Standard fullforwardshadows
		#pragma target 3.0
		
		ENDCG
	}
}
