//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
struct appdata
{
	float4 vertex	: POSITION;
	float2 uv		: TEXCOORD0;
	float4 color	: Color;
};

struct v2f
{
	float2 uv		: TEXCOORD0;
	float4 color	: Color;
	float4 vertex	: SV_POSITION;
};

sampler2D	_MainTex;
float4		_MainTex_ST;

fixed4		_Color;			

v2f vert (appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.color = v.color;
	return o;
}

fixed4 frag_image (v2f i) : SV_Target
{
	fixed4 texCol = tex2D(_MainTex, i.uv);
	return texCol * _Color * i.color;
}

fixed4 frag_text (v2f i) : SV_Target
{
	fixed4 texCol = tex2D(_MainTex, i.uv);
	texCol.rgb = fixed3(1,1,1);
	return texCol * _Color * i.color;
}