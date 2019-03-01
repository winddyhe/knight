Shader "PostEffect/Bloom"
{
	Properties
	{
		_MainTex ("MainTex", 2D)				= "white" {}
		_BlurSize("BlurSize",Range(0,10))		=1
		_LuminanceThreshold("阙值",Range(0,10))	=1
		_Bloom("Bloom",2D)						= "white"{}
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True"}
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex luminanceVert
			#pragma fragment luminanceFrag
			
			#include "UnityCG.cginc"
			#include "BlurEffectCG.cginc"
			float _LuminanceThreshold;
			bloomV2f luminanceVert(appdata a)
			{
				bloomV2f o;
				o.vertex=UnityObjectToClipPos(a.vertex);
				o.uv.xy=TRANSFORM_TEX(a.uv,_MainTex);
				o.uv.z = 0;
				o.uv.w = 0;
				return o;
			}
			fixed4 luminanceFrag(bloomV2f i):SV_Target
			{
				fixed4 col= tex2D(_MainTex,i.uv.xy);
				fixed val=luminance(col);
				val= clamp(val-_LuminanceThreshold,0,1);
				return col*val;
			}
			ENDCG
		}
		UsePass "PostEffect/GaussinBlur/HORIZONTAL_BLUR"
		UsePass "PostEffect/GaussinBlur/VERTICAL_BLUR"
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "BlurEffectCG.cginc"
			sampler2D	_Bloom;
			fixed4		_Bloom_ST;
			bloomV2f vert(appdata a)
			{
				bloomV2f o;
				o.vertex=UnityObjectToClipPos(a.vertex);
				o.uv.xy	=TRANSFORM_TEX(a.uv,_MainTex);
				o.uv.zw	=TRANSFORM_TEX(a.uv,_Bloom);
				return o;
			}
			fixed4 frag(bloomV2f i):SV_Target
			{
				fixed4 col		=tex2D(_MainTex,i.uv.xy);
				fixed4 bloomCol	=tex2D(_Bloom,i.uv.zw);
				return col+bloomCol;
			}
			ENDCG
		}
	}
}
