//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
Shader "WindShaderLab/Diffuse/ColorBlendCutoff"
{
	Properties
	{
		_MainTex	("Texture", 2D)					= "white" {}
		_BlendColor ("Blend Color", Color)			= (1.0, 1.0, 1.0, 1.0)
		_CutOff		("Cutoff factor", Range(0, 1))	= 0.5

		[Enum(Off,2 ,On,0)]	_CullMode("双面", float) = 0
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		Fog  { Color(1.0, 1.0, 1.0, 1.0) }

		Cull  [_CullMode]
		
		Pass
		{
			CGPROGRAM
			#pragma vertex	 vert
			#pragma fragment frag_cutoff
			
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			#include "ColorBlend.cginc"

			ENDCG
		}
	}
}
