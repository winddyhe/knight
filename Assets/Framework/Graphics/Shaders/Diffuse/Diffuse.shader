Shader "WindShaderLab/Diffuse/Diffuse" 
{
	Properties 
	{
		_Color	 ("Color", Color)	  = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}

		[Enum(UnityEngine.Rendering.CullMode)] _Cull("双面",float) = 1
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		Fog  { Color(1.0, 1.0, 1.0, 1.0) }

		Cull[_Cull]
		
		CGPROGRAM
		#include "Diffuse.cginc"
		#pragma surface SimpleDiffuse Standard fullforwardshadows
		
		ENDCG
	}
}
