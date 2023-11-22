Shader"Custom/CustomLight"
{
    Properties
    {
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("NormalMap", 2D) = "bump"{}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Test 

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));//노멀맵 추가
            o.Alpha = c.a;
        }
float4 LightingTest(SurfaceOutput s, float3 lightDir, float atten)
{
    float ndotl = saturate(dot(s.Normal, lightDir)); //0~1사이, 램버트
    float4 final;
    final.rgb = ndotl * s.Albedo * _LightColor0.rgb * atten; //Albedo : 텍스쳐, _LightColor0.rgb : 조명의 색상
    final.a = s.Alpha;
    return final;
}
        ENDCG
    }
    FallBack "Diffuse"
}
