// Upgrade NOTE: replaced 'defined ADDITIVI_COLOR_ADD' with 'defined (ADDITIVI_COLOR_ADD)'

#include"UnityCG.cginc"

sampler2D		_FirstTex;
sampler2D		_SecondTex;
sampler2D		_ThirdTex;
uniform half4	_FirstColor;
uniform half4	_SecondColor;
uniform half4	_ThirdColor;
uniform float4	_FirstTex_ST;
uniform float4	_SecondTex_ST;
uniform float4	_ThirdTex_ST;
uniform float	_FirstSpeed;
uniform float	_FirstAngel;
uniform float	_SecondSpeed;
uniform float	_SecondAngel;
uniform float	_ThirdSpeed;
uniform float	_ThirdAngel;
uniform fixed4	_Color;

struct Input
{
	float4 vert	:POSITION;
	half4 uv1	:TEXCOORD0;
};
struct v2f
{
	float4 vert	:POSITION;
	half2 uv1	:TEXCOORD0;
	half2 uv2	:TEXCOORD1;
	half2 uv3	:TEXCOORD2;
};
half2 TexMulRotate(float angel,float speed,half2 uv)
{
	float totalAngel		=angel*speed;
	fixed2x2 rotateMatrix	=fixed2x2(cos(totalAngel),-sin(totalAngel),sin(totalAngel),cos(totalAngel));
	half2 texUV				=mul(uv-fixed2(0.5,0.5),rotateMatrix)+fixed2(0.5,0.5); 

	return texUV;
}
v2f SetFirstTexUV(Input IN)
{
	v2f o;
	o.vert	=UnityObjectToClipPos(IN.vert);
	o.uv1	=TRANSFORM_TEX(IN.uv1,_FirstTex);
	o.uv2	=TRANSFORM_TEX(IN.uv1,_SecondTex);
	o.uv3	=TRANSFORM_TEX(IN.uv1,_ThirdTex);

	#ifdef FLIP_UPDOWN_OFF
		o.uv1.y=o.uv1.y;
	#else
		o.uv1.y=1-o.uv1.y;
	#endif
	 
	#ifdef FLIP_LEFTRIGHT_OFF
		o.uv1.x=o.uv1.x;
	#else
		o.uv1.x=1-o.uv1.x;
	#endif
	o.uv1=TexMulRotate(_FirstAngel,_FirstSpeed,o.uv1); 
	return o;
}
v2f SetSecondTexUV(v2f o)
{
	#ifdef SECOND_FLIP_UPDOWN_OFF
		o.uv2.y=o.uv2.y;
	#else
		o.uv2.y=1-o.uv2.y;
	#endif
	#ifdef SECOND_FLIP_LEFTRIGHT_OFF
		o.uv2.x=o.uv2.x;
	#else
		o.uv2.x=1-o.uv2.x;
	#endif
	o.uv2= TexMulRotate(_SecondAngel,_SecondSpeed,o.uv2);
	return o;
}
v2f Firstvert(Input IN)
{
	return SetFirstTexUV(IN);
}
v2f Secondvert(Input IN)
{
	v2f v;
	v=SetFirstTexUV(IN);
	return SetSecondTexUV(v);
}
v2f ThirdVert(Input IN)
{
	v2f v;
	v=SetFirstTexUV(IN);
	v=SetSecondTexUV(v);
	#ifdef THIRD_FLIP_UPDOWN_OFF
		v.uv3.y=v.uv3.y;
	#else
		v.uv3.y=1-v.uv3.y;
	#endif
	#ifdef THIRD_FLIP_LEFTRIGHT_OFF
		v.uv3.x=v.uv3.x;
	#else
		v.uv3.x=1-v.uv3.x;
	#endif
	v.uv3= TexMulRotate(_ThirdAngel,_ThirdSpeed,v.uv3);
	return v;
}



fixed4 Firstfrag(v2f v):COLOR
{
	return tex2D(_FirstTex,v.uv1)*_Color;
}
fixed4 Secondfrag(v2f v):COLOR
{
	#ifdef ADDITIVI_COLOR_OFF
		return tex2D(_FirstTex,v.uv1)*_FirstColor+tex2D(_SecondTex,v.uv2)*_SecondColor;
	#else
		return tex2D(_FirstTex,v.uv1)*_FirstColor*tex2D(_SecondTex,v.uv2)*_SecondColor;
	#endif
}
fixed4 Thirdfrag(v2f v):COLOR
{
	#ifdef ADDITIVI_COLOR_OFF
		return tex2D(_FirstTex,v.uv1)*_FirstColor+tex2D(_SecondTex,v.uv2)*_SecondColor+tex2D(_ThirdTex,v.uv3)*_ThirdColor;
	#else
		return tex2D(_FirstTex,v.uv1)*_FirstColor*tex2D(_SecondTex,v.uv2)*_SecondColor*tex2D(_ThirdTex,v.uv3)*_ThirdColor;
	#endif
}


