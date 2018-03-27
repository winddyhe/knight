Shader "WindShaderLab/PostEffect/RapidBlurEffect"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		ZWrite Off
		Blend Off

		Pass
		{
			ZTest Off
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert_DownSmpl
			#pragma fragment frag_DownSmpl
			ENDCG
		}

		Pass
		{
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vert_BlurVertical
			#pragma fragment frag_Blur
			ENDCG
		}
		
		Pass
		{
			ZTest Always
			Cull Off

			CGPROGRAM
			#pragma vertex vert_BlurHorizontal
			#pragma fragment frag_Blur
			ENDCG
		}
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D		_MainTex;
	uniform half4	_MainTex_TexelSize;
	uniform half	_DownSampleValue;

	struct VertexInput
	{
		float4 vertex	: POSITION;
		half2 texcoord	: TEXCOORD0;
	};

	struct VertexOutput_DownSmpl
	{
		float4 pos		: SV_POSITION;
		half2 uv20		: TEXCOORD0;
		half2 uv21		: TEXCOORD1;
		half2 uv22		: TEXCOORD2;
		half2 uv23		: TEXCOORD3;
	};


	// 准备高斯模糊权重矩阵参数7x4的矩阵 ||  Gauss Weight
	static const half4 GaussWeight[7] =
	{
		half4(0.0205,0.0205,0.0205,0),
		half4(0.0855,0.0855,0.0855,0),
		half4(0.232,0.232,0.232,0),
		half4(0.324,0.324,0.324,1),
		half4(0.232,0.232,0.232,0),
		half4(0.0855,0.0855,0.0855,0),
		half4(0.0205,0.0205,0.0205,0)
	};


	//【6】顶点着色函数 || Vertex Shader Function
	VertexOutput_DownSmpl vert_DownSmpl(VertexInput v)
	{
		VertexOutput_DownSmpl o;

		o.pos = UnityObjectToClipPos(v.vertex);
		//对图像的降采样：取像素上下左右周围的点，分别存于四级纹理坐标中
		o.uv20 = v.texcoord + _MainTex_TexelSize.xy* half2(0.5h, 0.5h);;
		o.uv21 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, -0.5h);
		o.uv22 = v.texcoord + _MainTex_TexelSize.xy * half2(0.5h, -0.5h);
		o.uv23 = v.texcoord + _MainTex_TexelSize.xy * half2(-0.5h, 0.5h);

		return o;
	}

	fixed4 frag_DownSmpl(VertexOutput_DownSmpl i) : SV_Target
	{
		fixed4 color = (0,0,0,0);
		color += tex2D(_MainTex, i.uv20);
		color += tex2D(_MainTex, i.uv21);
		color += tex2D(_MainTex, i.uv22);
		color += tex2D(_MainTex, i.uv23);

		return color / 4;
	}

	struct VertexOutput_Blur
	{
		float4 pos		: SV_POSITION;
		half4 uv		: TEXCOORD0;
		half2 offset	: TEXCOORD1;
	};

	VertexOutput_Blur vert_BlurHorizontal(VertexInput v)
	{
		VertexOutput_Blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = half4(v.texcoord.xy, 1, 1);
		o.offset = _MainTex_TexelSize.xy * half2(1.0, 0.0) * _DownSampleValue;
		return o;
	}

	VertexOutput_Blur vert_BlurVertical(VertexInput v)
	{
		VertexOutput_Blur o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = half4(v.texcoord.xy, 1, 1);
		o.offset = _MainTex_TexelSize.xy * half2(0.0, 1.0) * _DownSampleValue;
		return o;
	}

	half4 frag_Blur(VertexOutput_Blur i) : SV_Target
	{
		half2 uv = i.uv.xy;
		half2 OffsetWidth = i.offset;
		half2 uv_withOffset = uv - OffsetWidth * 3.0;

		half4 color = 0;
		for (int j = 0; j< 7; j++)
		{
			half4 texCol = tex2D(_MainTex, uv_withOffset);
			color += texCol * GaussWeight[j];
			uv_withOffset += OffsetWidth;
		}
		return color;
	}
	ENDCG
	FallBack Off
}