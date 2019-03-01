//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
struct appdata
{
	float4 vertex	: POSITION;
	half2  uv		: TEXCOORD0;
};

struct v2f
{
	half2 uv		: TEXCOORD0;
	UNITY_FOG_COORDS(1)
	float4 vertex	: SV_POSITION;
};

sampler2D	_MainTex;
half4		_MainTex_ST;

fixed4		_BlendColor;

float		_CutOff;

v2f vert (appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

fixed4 frag (v2f i) : SV_Target
{
	fixed4 mainCol = tex2D(_MainTex, i.uv);
	fixed4 col = fixed4(lerp(mainCol.rgb, _BlendColor.rgb, _BlendColor.a), mainCol.a);

	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}

fixed4 frag_cutoff (v2f i) : SV_Target
{
	fixed4 mainCol = tex2D(_MainTex, i.uv);
	fixed4 col = fixed4(lerp(mainCol.rgb, _BlendColor.rgb, _BlendColor.a), mainCol.a);
	clip(col.a - _CutOff);

	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}