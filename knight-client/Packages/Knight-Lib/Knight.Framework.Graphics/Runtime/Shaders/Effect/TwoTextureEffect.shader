Shader "Texture/TwoTextureEffect" 
{
	Properties
	{
		_FirstTex("第一张贴图", 2D)					= "white"{}
		_SecondTex("第二张贴图", 2D)					= "white"{}
		_FirstColor("第一个Color通道", COLOR)		= (1,1,1,1)
		_SecondColor("第二个Color通道", COLOR)		= (1,1,1,1)
		_FirstSpeed("第一张图旋转速度", float)		= 0.5
		_SecondSpeed("第二章图旋转速度", float)		= 0.5
		_FirstAngel("第一张图旋转角度",Range(0,360))	=30
		_SecondAngel("第二章图旋转角度",Range(0,360))	=30

		[Toggle] SECOND_FLIP_UPDOWN("第二张图上下翻转",float)				= 0
		[Toggle] SECOND_FLIP_LEFTRIGHT("第二张图左右翻转",float)			= 0
		[Toggle] FLIP_UPDOWN("第一张图上下翻转",float)					= 0
		[Toggle] FLIP_LEFTRIGHT("第一张图左右翻转",float)					= 0
		[Toggle] ADDITIVI_COLOR("两张图颜色相加",float)					= 0
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("源图",float)	= 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DesBlend("目标图",float)	= 0
		
	}

	SubShader
	{
		Tags{"RenderType"="Opaque"}
		Cull Off
		Blend[_SrcBlend][_DesBlend]
		Pass
		{
			CGPROGRAM

			#pragma vertex   Secondvert
			#pragma fragment Secondfrag
			
			#pragma multi_compile SECOND_FLIP_UPDOWN_OFF SECOND_FLIP_UPDOWN_ON 
			#pragma multi_compile SECOND_FLIP_LEFTRIGHT_OFF SECOND_FLIP_LEFTRIGHT_ON 
			#pragma multi_compile ADDITIVI_COLOR_OFF ADDITIVI_COLOR_ON
			#pragma multi_compile FLIP_UPDOWN_OFF	 FLIP_UPDOWN_ON 
			#pragma multi_compile FLIP_LEFTRIGHT_OFF FLIP_LEFTRIGHT_ON 

			#include "Effect.cginc"

			ENDCG
		}
	}
}