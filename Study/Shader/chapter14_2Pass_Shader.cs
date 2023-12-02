Shader "Custom/toon"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        cull front//뒷면이 보이게

//1st pass
        CGPROGRAM
        #pragma surface surf Nolight vertex:vert noshadow noambient
        #pragma target 3.0
        
void vert(inout appdata_full v)//버텍스 셰이더
{
    v.vertex.xyz = v.vertex.xyz + v.normal.xyz * 0.01;
}

struct Input
{
    float4 color : COLOR;
};
void surf(Input IN, inout SurfaceOutput o)
{
}
float4 LightingNolight(SurfaceOutput s, float3 lightDir, float atten)
{
    return float4(0, 0, 0, 1);
}
ENDCG

cull back//앞면이 보이게

//2nd pass
CGPROGRAM
        #pragma surface surf Lambert
        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
