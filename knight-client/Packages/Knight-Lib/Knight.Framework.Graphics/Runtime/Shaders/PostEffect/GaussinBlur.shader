Shader "PostEffect/GaussinBlur"
{
	Properties
	{
		_MainTex ("MainTex", 2D)			= "white" {}
		_BlurSize("BlurSize",Range(0,10))	=1
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True"}
		Pass
		{
			NAME "HORIZONTAL_BLUR"
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex horizontalVert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "BlurEffectCG.cginc"

			ENDCG
		}
		Pass
		{
			NAME "VERTICAL_BLUR"
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex verticalVert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "BlurEffectCG.cginc"

			ENDCG
		}
	}
}
