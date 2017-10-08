Shader "PostEffect/SceneBoom"
{
	Properties
	{
		_MainTex ("MainTex", 2D)		= "white" {}
		_SatCount("SatCount",float)		=1
		_ScaleX("ScaleX",float)			=0.1
		_ScaleY("ScaleY",float)			=0.1
		_BumpTex("BumpTex",2D)			="white"{}
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 200
		Pass
		{
			Cull back ZWrite Off ZTest Always
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
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
				UNITY_FOG_COORDS(1)
			};
			sampler2D _MainTex;
			sampler2D _BumpTex;
			fixed4 _MainTex_ST;
			float _SatCount;
			float _ScaleX;
			float _ScaleY;
			v2f vert(Input i)
			{
				v2f o;
				o.vertex	=UnityObjectToClipPos(i.vertex);
				o.uv		=TRANSFORM_TEX(i.uv,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target
			{
				half2 bumpUV	=i.uv;
				bumpUV			+=float2(_ScaleX,_ScaleY);
				half2 bump		=UnpackNormal(tex2D(_BumpTex,bumpUV)).rg;
				i.uv			=bump+i.uv;//为什么这里把uv坐标加了一个法线值之后采样出来的图像就有法线了
				fixed4 col		=tex2D(_MainTex,i.uv);
				fixed4 lum		=Luminance(col);
				col				=lerp(col,lum,_SatCount);
				return col;
			}
			ENDCG
		}
	}
}
