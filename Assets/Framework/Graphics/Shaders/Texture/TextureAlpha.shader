Shader "WindShaderLab/Texture/TextureAlpha"
{
	Properties
	{
		_MainTex ("Main Texture", 2D)  = "white" {}
		_Color   ("Main Color", color) = (1.0, 1.0, 1.0, 1.0)
		
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc("BlendSrc", float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDes("BlendDes", float) = 1
        [Enum(Off,2 ,On,0)] _CullMode("双面", Float)  = 0
        [Toggle] _ZWriteMode("深度", Float) = 1
	}

	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType" = "Transparent" }
        Fog  { Mode Off }
		
		Blend  [_BlendSrc][_BlendDes]
        Cull   [_CullMode]
		ZWrite [_ZWriteMode]

		Pass
		{
			CGPROGRAM

			#pragma vertex	 vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "Texture.cginc"

			ENDCG
		}
	}
}
