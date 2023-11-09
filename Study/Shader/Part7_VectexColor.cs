Shader "Custom/VCMask"
{
    Properties
    {
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
_MainTex3 ("Albedo (RGB)", 2D) = "white" {}
_MainTex4 ("Albedo (RGB)", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard noambient

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
sampler2D _MainTex2;
sampler2D _MainTex3;
sampler2D _MainTex4;

        struct Input
        {
            float2 uv_MainTex;
    float2 uv_MainTex2;
    float2 uv_MainTex3;
    float2 uv_MainTex4;
            float4 color : COLOR;
        };

        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
    fixed4 d = tex2D(_MainTex2, IN.uv_MainTex2);
    fixed4 e = tex2D(_MainTex3, IN.uv_MainTex3);
    fixed4 f = tex2D(_MainTex4, IN.uv_MainTex4);
    o.Albedo = lerp(c.rgb,d.rgb,IN.color.r);
    o.Albedo = lerp(o.Albedo,e.rgb, IN.color.g);
    o.Albedo = lerp(o.Albedo,f.rgb, IN.color.b);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
