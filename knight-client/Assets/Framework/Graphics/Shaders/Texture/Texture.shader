Shader "WindShaderLab/Texture/Texture"
{
	Properties
	{
		_MainTex ("Main Texture", 2D)  = "white" {}
		_Color   ("Main Color", color) = (1.0, 1.0, 1.0, 1.0)

        [Enum(Off,2 ,On,0)] CullMode("双面", Float)  = 0
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		
        Cull   [CullMode]
        Fog    { Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex	 vert
			#pragma fragment frag

			#pragma multi_compile _VERTEX_COLOR_OFF _VERTEX_COLOR_ON
			
			#include "UnityCG.cginc"
			#include "Texture.cginc"

			ENDCG
		}
	}
}
