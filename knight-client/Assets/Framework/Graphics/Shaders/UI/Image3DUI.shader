//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
Shader "WindShaderLab/3DUI/Image3DUI"
{
	Properties
	{
		_MainTex ("Texture", 2D ) = "white" {}
		_Color	 ("Color", Color) = (1, 1, 1, 1)
	}

	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		
		Blend  SrcAlpha OneMinusSrcAlpha
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma vertex	 vert
			#pragma fragment frag_image
			
			#include "UnityCG.cginc"
			#include "ThreeDUI.cginc"

			ENDCG
		}
	}
}
