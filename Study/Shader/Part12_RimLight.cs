Shader"Custom/RimLight"
{
    Properties
    {
       
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _BumpMap("NormalMap",2D) = "bump"{}
        _RimColor("RimColor", Color) = (1,1,1,1)
        _RimPower("RimPower",Range(1,10)) = 3
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert 

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        float4 _RimColor;
        float _RimPower;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float3 viewDir;//뷰 벡터, 카메라 방향
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
    float rim = saturate(dot(o.Normal, IN.viewDir)); //카메라 벡터와 내적
    o.Emission = pow(1 -rim, _RimPower)*_RimColor.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
