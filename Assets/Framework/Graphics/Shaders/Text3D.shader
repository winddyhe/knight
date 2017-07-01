//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
Shader "WindShaderLab/UI/3DUI"
{
	Properties
	{
		_MainTex ("Texture", 2D ) = "white" {}
		_Color	 ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Blend  SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D	_MainTex;
			float4		_MainTex_ST;

			fixed4		_Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 texCol = tex2D(_MainTex, i.uv);
				return texCol * _Color;
			}
			ENDCG
		}
	}
}
