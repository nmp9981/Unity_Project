Shader"Custom/Refraction"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _RefStrength("Reflection Strength",Range(0,0.1)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        zwrite off//앞의 쿼드는 나중에 그려져야 함
        LOD 200

GrabPass//실시간 화면 캡처
{
}
        CGPROGRAM
        #pragma surface surf nolight noambient alpha:fade
        #pragma target 3.0

sampler2D _GrabTexture;//화면 캡쳐 결과
sampler2D _MainTex;
float _RefStrength;

struct Input
{
    float4 color : COLOR;
float4 screenPos;//현재 화면의 uv
    float2 uv_MainTex;
};

        void surf (Input IN, inout SurfaceOutput o)
{
    float4 ref = tex2D(_MainTex, IN.uv_MainTex);
    float3 screenUV = IN.screenPos.rgb / IN.screenPos.a; //카메라의 거리 영향 제거, 화면 uv설정
    o.Emission = tex2D(_GrabTexture, screenUV.xy+ref.x*_RefStrength);
}
float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
{
    return float4(0, 0, 0, 1);
}
        ENDCG
    }
    FallBack "Regacy Shaders/Transparent/Vertexlit"
}
