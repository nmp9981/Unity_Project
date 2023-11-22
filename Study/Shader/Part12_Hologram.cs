Shader "Custom/Hologram"
{
    Properties
    {
        _BumpMap("NormalMap",2D) = "bump"{}
        
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 300

        CGPROGRAM
        #pragma surface surf nolight noambient alpha : fade

        #pragma target 3.0

sampler2D _MainTex;
sampler2D _BumpMap;

struct Input
{
    float2 uv_BumpMap;
    float3 viewDir; //뷰 벡터, 카메라 방향
    float3 worldPos;
};

void surf(Input IN, inout SurfaceOutput o)
{
    //o.Albedo = c.rgb;
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
    o.Emission = float3(0, 1, 0);//홀로그램 색상
    float rim = saturate(dot(o.Normal, IN.viewDir)); //카메라 벡터와 내적
    rim = saturate(pow(1 - rim, 3) + pow(frac(IN.worldPos.g * 3 - _Time.y), 5))*0.7;
    o.Alpha = rim;
}
float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
{
    return float4(0, 0, 0, s.Alpha);
}
        ENDCG
    }
    FallBack "Diffuse"
}
