Shader "Optimize/OptimizeDiffuse" {
	Properties{
		_MainTex("MainTex",2D)="white"{}
		_Color("COLOR",COLOR)=(0,0,0,0)
	}
	SubShader{
		Tags{"RenderType"="Opaque"}
		LOD 100
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				fixed4 vert:POSITION;
				fixed4 uv:TEXCOORD0;
			};
			struct v2f
			{
				fixed4 Pos:POSITION;
				fixed4 uv:TEXCOORD;
			};
			sampler2D _MainTex;
			uniform fixed4 _Color;
			v2f vert(appdata data)
			{
				v2f o;
				o.Pos=UnityObjectToClipPos(data.vert);
				o.uv=data.uv;
				return o;
			}
			fixed4 frag(v2f v):COLOR
			{
				return tex2D(_MainTex,v.uv)*_Color; 
			}
			ENDCG
		}
		
	}
}
