Shader "WindShaderLab/Diffuse/DiffuseCutOff" 
{
	Properties 
	{
		_Color   ("Color", Color)      = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D)  = "white" {}
		_CutOff  ("CutOff",Range(0,1)) = 0.5

		[Enum(UnityEngine.Rendering.CullMode)] _Cull("双面",float) = 1
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Fog  { Color(1.0, 1.0, 1.0, 1.0) }

		Cull[_Cull]
		
		CGPROGRAM
		#pragma surface SimpleDiffuseCutOff Standard fullforwardshadows
		#include "Diffuse.cginc"
		
		ENDCG
	}
}
