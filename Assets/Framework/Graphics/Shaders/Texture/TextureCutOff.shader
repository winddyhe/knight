Shader "WindShaderLab/Texture/TextureCutOff"
{
	Properties
	{
		_MainTex ("Main Texture", 2D)	  = "white" {}
		_Color   ("Main Color", color)	  = (1.0, 1.0, 1.0, 1.0)
		_CutOff  ("Cut Off", Range(0, 1)) = 0.5
		
        [Enum(Off,2 ,On,0)] CullMode("双面", Float)  = 0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
        Fog  { Mode Off }
		
        Cull  [CullMode]

		Pass
		{
			CGPROGRAM

			#pragma vertex	 vert
			#pragma fragment frag_cutoff

			#include "UnityCG.cginc"
			#include "Texture.cginc"

			ENDCG
		}
	}
}
