Shader "Optimize/OptimizeDiffusealpha" {
	Properties{
		_MainTex("MainTex",2D)="white"{}
		_Color("COLOR",COLOR)=(0,0,0,0)
		_CutOff("CutOff",Range(0,1))=0.5  
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendSrc("BlendSrc",float)=2
		[Enum(UnityEngine.Rendering.BlendMode)]_BlendDes("BlendDes",float)=2
	}
	SubShader
	{
		
		Tags{"Queue"="Transparent" "RenderType"="Transparent"}
		LOD 100
		
		Pass
		{
			Blend[_BlendSrc][_BlendDes]
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
				fixed4 uv:TEXCOORD0;
			};
			sampler2D _MainTex;
			uniform fixed4 _Color;
			uniform float _CutOff;
			v2f vert(appdata data)
			{
				v2f o;
				o.Pos=UnityObjectToClipPos(data.vert);
				o.uv=data.uv;
				return o;
			}
			fixed4 frag(v2f vertexData):COLOR
			{
				fixed4 col= tex2D(_MainTex,vertexData.uv)*_Color;
				clip(col.a-_CutOff);
				return col;
			}
			ENDCG
		}
	}
	FallBack "Optimize/OptimizeDiffuse"
}
