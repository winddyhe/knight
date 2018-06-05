
#include"UnityCG.cginc"

sampler2D	_MainTex;

fixed4		_Color;
float		_CutOff;

struct Input
{
	half2 uv_MainTex;
};

inline void SimpleDiffuse(Input IN, inout SurfaceOutputStandard o)
{
    fixed4 mainCol=tex2D(_MainTex,IN.uv_MainTex);
	fixed4 col = fixed4(lerp(mainCol.rgb, _Color.rgb, _Color.a), mainCol.a);
	o.Albedo =col.rgb;
	o.Alpha=col.a;
}

inline void SimpleDiffuseCutOff(Input IN, inout SurfaceOutputStandard o)
{
	fixed4 mainCol = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 col = fixed4(lerp(mainCol.rgb, _Color.rgb, _Color.a), mainCol.a);
	clip(col.a - _CutOff);
	o.Albedo =col.rgb;
	o.Alpha=col.a;
}