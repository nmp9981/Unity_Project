Shader"Custom/Reflection"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
_BumpMap("NormalMap",2D) = "bump"{}
_MaskMap("MaskMap",2D) = "white"{}//마스크 맵
        _Cube("Cubemap", Cube) = ""{}//큐브 텍스처
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        CGPROGRAM
        #pragma surface surf Lambert 

        sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _MaskMap;
        samplerCUBE _Cube;//큐브 맵

        struct Input
        {
            float2 uv_MainTex;
    float2 uv_BumpMap;
    float2 uv_MaskMap;
            float3 worldRefl;//반사 벡터
    INTERNAL_DATA//버텍스 노멀데이터를 픽셀 노멀데이터로 변환 시키기위한 행렬이 가동됨
};

        void surf (Input IN, inout SurfaceOutput o)
        {
    o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
    float4 re = texCUBE(_Cube, WorldReflectionVector(IN, o.Normal));//노멀맵이 적용된 월드 좌표계의 픽셀 노멀을 뽑아서 큐브맵의 UV로 사용
    float4 m = tex2D(_MaskMap, IN.uv_MaskMap);
    
    o.Albedo = c.rgb * (1 - m.r);
            o.Emission = re.rgb*m.r;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
