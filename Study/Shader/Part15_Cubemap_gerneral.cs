Shader"Custom/Reflection"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cube("Cubemap", Cube) = ""{}//큐브 텍스처
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Lambert noambient

        sampler2D _MainTex;
        samplerCUBE _Cube;//큐브 맵

        struct Input
        {
            float2 uv_MainTex;
            float3 worldRefl;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            float4 re = texCUBE(_Cube, IN.worldRefl);
            o.Albedo = 0;
            o.Emission = re.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
