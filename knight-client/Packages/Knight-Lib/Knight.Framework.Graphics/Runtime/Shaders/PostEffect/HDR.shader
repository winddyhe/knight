Shader "PostEffect/HDR"
{
	Properties
	{
		_MainTex ("MainTex", 2D)			= "white" {}
		_Color("Color",COLOR)				=(1,1,1,1)
		_BrightPow("Bright Pow",Range(0,4))	=1
		_GamaPow("GamaPow",Range(0,4))		=1
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
			float		_BrightPow;
			float		_GamaPow;
			fixed4		_Color;
			v2f vert(Input i)
			{
				v2f o;
				o.vertex	=UnityObjectToClipPos(i.vertex);
				o.uv		=TRANSFORM_TEX(i.uv,_MainTex);
				return o;
			}
			fixed HDRPower(fixed3 InColor)
			{
				fixed result	=min(InColor.r,min(InColor.g,InColor.b))+max(InColor.r,max(InColor.g,InColor.b));
				result			=1-result*0.5;
				return exp(-(result*result));
			}
			fixed HDRPower(fixed3 InColor,fixed3 NormalColor)
			{
				fixed result0		=min(InColor.r,min(InColor.g,InColor.b))+max(InColor.r,max(InColor.g,InColor.b));
				fixed resultNormal	=min(NormalColor.r,min(NormalColor.g,NormalColor.b))+max(NormalColor.r,max(NormalColor.g,NormalColor.b));
				fixed result		=(resultNormal-result0)*0.5;
				return exp(-(result*result));
			}
			fixed4 frag(v2f i):SV_Target
			{
				float4 col	=tex2D(_MainTex,i.uv);
				float y		=dot(fixed4(0.3,0.59,0.11,1),col);
				float yd	=_BrightPow*(_BrightPow/_GamaPow+1)/(_BrightPow+1);
				return col*yd;
			}
			//另外一个版本的HDR算法
			//fixed4 frag(v2f i):SV_Target
			//{
			//	fixed4 col=tex2D(_MainTex,i.uv)*_Color;
			//	fixed3 BrightRGB=saturate(col.rgb*_BrightPow);
			//	fixed3 DimRGB=pow(col.rgb,_GamaPow);
			//	fixed3 FinalRGB=fixed3(0,0,0);
			//	fixed BrightHDRPow=HDRPower(BrightRGB,col.rgb);
			//	fixed DimHDRPow=HDRPower(DimRGB,col.rgb);
			//	fixed NormalHDRPow=HDRPower(col.rgb);
			//	FinalRGB=(BrightRGB*BrightHDRPow+DimRGB*DimHDRPow+NormalHDRPow*col);
			//	return  fixed4(FinalRGB,1);
			//}
			ENDCG
		}
	}
}
