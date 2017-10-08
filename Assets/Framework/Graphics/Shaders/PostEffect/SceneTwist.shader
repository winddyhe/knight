Shader "PostEffect/SceneTwist"
{
	Properties
	{
		_MainTex ("MainTex", 2D)	= "white" {}
		_Twist("Twist",float)		=1
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
			fixed4 _MainTex_ST;
			fixed2 _MainTex_TexelSize;
			float _Twist;
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
				fixed2 twistuv	=i.uv;
				fixed2 uv		=fixed2(twistuv.x-0.5,twistuv.y-0.5);
				float angel		=_Twist*0.1745/(length(uv)+0.1);
				fixed sinval,cosval;
				sincos(angel,sinval,cosval);
				float2x2 mat	=float2x2(cosval,-sinval,sinval,cosval);
				uv				=mul(mat,uv)+0.5;
				fixed4 col		=tex2D(_MainTex,uv);
				UNITY_APPLY_FOG(i.fogCoord, col);  
				return col;
			}
			ENDCG
		}
	}
}
