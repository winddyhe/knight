Shader "Texture/TextureEffect" 
{
	Properties
	{
		_FirstTex("第一张贴图", 2D)					= "white"{}
		_Color("MianColor", COLOR)					= (1,1,1,1)
		_FirstSpeed("旋转速度",float)				=0.5
		_FirstAngel("第一张图旋转角度",Range(0,360))	=30

		[Toggle] FLIP_UPDOWN("上下翻转",float)		= 0
		[Toggle] FLIP_LEFTRIGHT("左右翻转",float)	= 0
	}

	SubShader
	{
		Tags{"RenderType"="Opaque"}
		Cull Off

		Pass
		{
			CGPROGRAM

			#pragma vertex   Firstvert
			#pragma fragment Firstfrag
			
			#pragma multi_compile FLIP_UPDOWN_OFF	 FLIP_UPDOWN_ON 
			#pragma multi_compile FLIP_LEFTRIGHT_OFF FLIP_LEFTRIGHT_ON 
			#include "Effect.cginc"
			
			ENDCG
		}
	}
}