struct appdata
{
	float4 vertex	: POSITION;
	float2 uv		: TEXCOORD0;
};

struct v2f
{
	float2 uv[5]	: TEXCOORD0;
	float4 vertex	: SV_POSITION;
};
struct bloomV2f
{
	float4 uv:TEXCOORD0;
	float4 vertex:SV_POSITION;
};
sampler2D	_MainTex;
fixed4		_MainTex_ST;
fixed4		_MainTex_TexelSize;
float		_BlurSize;
fixed luminance(fixed4 color)
{
	return 0.2125*color.r+0.7154*color.g+0.0721*color.b;
}
v2f horizontalVert (appdata v)
{
	v2f o;
	o.vertex=UnityObjectToClipPos(v.vertex);
	o.uv[0]=v.uv;
	o.uv[1]=v.uv+float2(_MainTex_TexelSize.x*1.0f,0)*_BlurSize;
	o.uv[2]=v.uv+float2(_MainTex_TexelSize.x*-1,0)*_BlurSize;
	o.uv[3]=v.uv+float2(_MainTex_TexelSize.x*2,0)*_BlurSize;
	o.uv[4]=v.uv+float2(_MainTex_TexelSize.x*-2,0)*_BlurSize;
	
	return o;
}
fixed4 frag (v2f i) : SV_Target
{
	//float weight[5]={0.4026,0.2442,0.2442,0.0545,0.0545};
	fixed3 gradient=fixed3(0,0,0);
	for(int j=0; j<5;j++)
	{
		//gradient+=tex2D(_MainTex,i.uv[j])*weight[j];
		gradient+=tex2D(_MainTex,i.uv[j]);
	}
	//return fixed4(gradient,1);
	fixed3 rColor=saturate(gradient/5);
	return fixed4(rColor,1);
}
v2f verticalVert (appdata v)
{
	v2f o;
	o.vertex=UnityObjectToClipPos(v.vertex);
	o.uv[0]=v.uv;
	o.uv[1]=v.uv+half2(0,_MainTex_TexelSize.y*1)*_BlurSize;
	o.uv[2]=v.uv+half2(0,_MainTex_TexelSize.y*-1)*_BlurSize;
	o.uv[3]=v.uv+half2(0,_MainTex_TexelSize.y*2)*_BlurSize;
	o.uv[4]=v.uv+half2(0,_MainTex_TexelSize.y*-2)*_BlurSize;
	return o;
}