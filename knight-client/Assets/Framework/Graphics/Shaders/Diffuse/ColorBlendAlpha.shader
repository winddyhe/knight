//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
Shader "WindShaderLab/Diffuse/ColorBlendAlpha"
{
	Properties
	{
		_MainTex	("Texture", 2D)				= "white" {}
		_BlendColor ("Blend Color", Color)		= (1.0, 1.0, 1.0, 1.0)

		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("BlendSrc", float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDes("BlendDes", float) = 1
		[Enum(Off,2 ,On,0)]						_CullMode("双面",	  float) = 0
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
		Fog  { Color(1.0, 1.0, 1.0, 1.0) }

		Blend [_BlendSrc][_BlendDes]
		Cull  [_CullMode]
		
		Pass
		{
			CGPROGRAM
			#pragma vertex   vert
			#pragma fragment frag
			
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "ColorBlend.cginc"

			ENDCG
		}
	}
}
