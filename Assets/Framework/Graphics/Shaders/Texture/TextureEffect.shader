Shader "Texture/TextureEffect" 
{
	Properties
	{
		_MainTex("MainTex", 2D)	   = "white"{}
		_Color("MianColor", COLOR) = (1,1,1,1)
		_Speed("Speed", float)	   = 0.5

		[Toggle] FLIP_UPDOWN("FLIP_UPDOWN",float)	   = 0
		[Toggle] FLIP_LEFTRIGHT("FlipLeftRight",float) = 0
	}

	SubShader
	{
		Tags{"RenderType"="Opaque"}
		Cull Off

		Pass
		{
			CGPROGRAM

			#pragma vertex   vert
			#pragma fragment frag

			#pragma multi_compile FLIP_UPDOWN_OFF	 FLIP_UPDOWN_ON 
			#pragma multi_compile FLIP_LEFTRIGHT_OFF FLIP_LEFTRIGHT_ON 
			
			#include "UnityCG.cginc"

			struct Input
			{
				float4 vert	: POSITION;
				half4  uv	: TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vert	: POSITION;
				half2   uv	: TEXCOORD;
			};
			
			sampler2D		_MainTex;
			uniform float	_Flip;
			uniform float4	_MainTex_ST;
			uniform fixed4	_Color;
			uniform float	_Speed;
			
			v2f vert(Input IN)
			{
				v2f o;
				o.vert=UnityObjectToClipPos(IN.vert);
				o.uv=TRANSFORM_TEX(IN.uv,_MainTex);
				return o;
			}

			fixed4 frag(v2f v) : COLOR
			{
				float uvX=v.uv.x+_Time*_Speed;
				float uvY=v.uv.y;
				#ifdef FLIP_UPDOWN_ON
					uvY=v.uv.y>0.5?v.uv.y-0.5:v.uv.y+0.5;
				#endif
				#ifdef FLIP_LEFTRIGHT_ON
					uvX=uvX>0.5?uvX-0.5:uvX+0.5;
				#endif
				fixed2 uv=fixed2(uvX,uvY);
				fixed4 rcolor= tex2D(_MainTex,uv)*_Color;
				return rcolor;
			}
			ENDCG
		}
	}
}
