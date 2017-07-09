struct appdata
{
	float4 vertex	: POSITION;
	half2  uv		: TEXCOORD0;
	fixed4 color	: COLOR;
};

struct v2f
{
	half2  uv		: TEXCOORD0;
	float4 vertex	: SV_POSITION;
	fixed4 color	: Color;
};

sampler2D	_MainTex;
float4		_MainTex_ST;

fixed4		_Color;
float		_CutOff;

v2f vert (appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.color = v.color;
	return o;
}

fixed4 frag (v2f i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	return col * _Color * i.color;
}

fixed4 frag_cutoff (v2f i) : SV_Target
{
	fixed4 col = tex2D(_MainTex, i.uv);
	col = col * _Color * i.color;
	clip(col.a - _CutOff);
	return col;
}