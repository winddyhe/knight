Shader "PostEffect/RadialBlur"
{
	Properties
	{
		_MainTex ("MainTex", 2D)				= "white" {}
		_OffsetX("X方向的偏移",Range(0,10))		=1
		_OffsetY("Y方向的偏移",Range(0,10))		=1
		_BlurSize("BlurSize",Range(0,10))		=1
		_IterationNum("迭代次数",Range(0,10))	=3
	}
	SubShader
	{
		Tags{"RenderType"="Transparent" "DisableBatching"="True" "IgnoreProjector"="True"}
		LOD 200
		Pass
		{
			Cull Off ZWrite Off ZTest Always
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
			fixed2		_MainTex_TexelSize;
			float		_OffsetX;
			float		_OffsetY;
			float		_BlurSize;
			float		_IterationNum;
			v2f vert(Input i)
			{
				v2f o;
				o.vertex	=UnityObjectToClipPos(i.vertex);
				o.uv		=TRANSFORM_TEX(i.uv,_MainTex);
				
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target
			{
				fixed4 finalCol	=fixed4(0,0,0,0);
				float scale		=1;
				i.uv			-=half2(_OffsetX,_OffsetY);
				for(int j=0;j<_IterationNum;j++)
				{
					 finalCol	+=tex2D(_MainTex,i.uv*scale+half2(_OffsetX,_OffsetY));
					 scale		=1+j*_BlurSize*0.085;
				}
				return saturate(finalCol/_IterationNum);
			}
			ENDCG
		}
	}
}
