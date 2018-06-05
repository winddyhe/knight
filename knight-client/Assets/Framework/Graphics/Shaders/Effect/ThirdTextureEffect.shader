Shader "Texture/ThirdTextureEffect" 
{
	Properties
	{
		_FirstTex("第一张贴图", 2D)					= "white"{}
		_ThirdTex("第三张贴图", 2D)					= "white"{}
		_SecondTex("第二张贴图", 2D)					= "white"{}

		_FirstColor("第一个Color通道", COLOR)		= (1,1,1,1)
		_SecondColor("第二个Color通道", COLOR)		= (1,1,1,1)
		_ThirdColor("第三个Color通道", COLOR)		= (1,1,1,1)

		_FirstSpeed("第一张图旋转速度", float)		= 0.5
		_SecondSpeed("第二章图旋转速度", float)		= 0.5
		_ThirdSpeed("第三章图旋转速度", float)		= 0.5

		_FirstAngel("第一张图旋转角度",Range(0,360))	=30
		_SecondAngel("第二章图旋转角度",Range(0,360))	=30
		_ThirdAngel("第三章图旋转角度",Range(0,360))	=30

		[Toggle] Third_FLIP_UPDOWN("第三张图上下翻转",float)				= 0
		[Toggle] Third_FLIP_LEFTRIGHT("第三张图左右翻转",float)			= 0
		[Toggle] SECOND_FLIP_UPDOWN("第二张图上下翻转",float)				= 0
		[Toggle] SECOND_FLIP_LEFTRIGHT("第二张图左右翻转",float)			= 0
		[Toggle] FLIP_UPDOWN("第一张图上下翻转",float)					= 0
		[Toggle] FLIP_LEFTRIGHT("第一张图左右翻转",float)					= 0
		[Toggle] ADDITIVI_COLOR("三张图颜色相加",float)					= 0
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("源图",float)	= 0
		[Enum(UnityEngine.Rendering.BlendMode)]_DesBlend("目标图",float)	= 0
		[Enum(UnityEngine.Rendering.CullMode)] _Cull("双面",float)		= 1
	}

	SubShader
	{
		Tags{"RenderType"="Opaque"}
		Cull [_Cull]
		Blend[_SrcBlend][_DesBlend]
		Pass
		{
			CGPROGRAM

			#pragma vertex   ThirdVert
			#pragma fragment Thirdfrag
			
			#pragma multi_compile THIRD_FLIP_UPDOWN_OFF THIRD_FLIP_UPDOWN_ON 
			#pragma multi_compile THIRD_FLIP_LEFTRIGHT_OFF THIRD_FLIP_LEFTRIGHT_ON 
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