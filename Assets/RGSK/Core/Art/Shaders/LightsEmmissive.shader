Shader "Custom/RGSK Lights Emmissive" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) RefStrength (A)", 2D) = "white" {} 
	
	_Intensity ("_Intensity", float) = 0
}
SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma surface surf BlinnPhong

sampler2D _MainTex;

float4 _Color;
float _Intensity;

struct Input {
	float2 uv_MainTex;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) 
{
	half4 tex = tex2D(_MainTex, IN.uv_MainTex);
	half4 c = tex * _Color;
	
	o.Albedo =  c.rgb;
	o.Emission = o.Albedo * _Intensity;
}
ENDCG
}
	
FallBack "Reflective/VertexLit"
} 