//Goal Outline Shader:
//http://wiki.unity3d.com/index.php?title=Silhouette-Outlined_Diffuse

Shader "Custom/Outline" {
	Properties {
		_Color ("Main Color", Color) = (0.5, 0.5, 0.5, 1)
		_OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
		_Outline ("Outline Width", Range(0.0, 0.05)) = 0.005
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

CGINCLUDE
#include "UnityCG.cginc"

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
}

struct vertex2float {
	float4 pos : POSITION;
	float4 color : COLOR;
}

uniform float _Outline;
uniform float4 _OutlineColor;


	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
