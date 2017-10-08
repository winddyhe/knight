Shader "PostEffect/MotionBlur"
{
	Properties
	{
		_MainTex ("MainTex", 2D)				= "white" {}
		_BlurAmount("Blur Amount",Range(0,1))	=0.5
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 200
		Pass
		{
			Cull Off ZWrite Off ZTest Always
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			struct Input
			{
				float2 uv		:TEXCOORD0;
				float4 vertex	:POSITION;
			};
			struct v2f
			{
				float2 uv		:TEXCOORD0;
				float4 vertex	:SV_POSITION;
			};
			sampler2D	_MainTex;
			fixed4		_MainTex_ST;
			float		_BlurAmount;
			v2f vert(Input i)
			{
				v2f o;
				o.vertex	=UnityObjectToClipPos(i.vertex);
				o.uv		=TRANSFORM_TEX(i.uv,_MainTex);
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target
			{
				fixed3 col	= tex2D(_MainTex,i.uv).rgb;
				return fixed4(col,_BlurAmount);
			}
			ENDCG
		}
	}
}
