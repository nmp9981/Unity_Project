Shader "Custom/CustomBlending"
{
    Properties
    {
        _TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)//전체 제어 색상, 초기값은 회색으로   
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}//unity 내장 프로젝트에 반응하지 않는다
        zwrite off//z버퍼 끄기
        blend SrcAlpha One//블렌딩 옵션
        cull off//컬링 안함(양면 컬링)
        LOD 200

        CGPROGRAM
        //해당하는 추가 쉐이더를 생성하지 않는다.
        #pragma surface surf nolight keepalpha noforwardadd nolightmap noambient novertexlights noshadow

        #pragma target 3.0

        sampler2D _MainTex;
        float4 _TintColor;

        struct Input
        {
            float2 uv_MainTex;
            float4 color : COLOR;//vertex color를 받아옴
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            c = c * 2 * _TintColor * IN.color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        //연산이 없는 라이팅 구문
        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0, 0, 0, s.Alpha);
        }
        ENDCG
    }
    FallBack "Legacy Shaders/Transparent/VertexLit"
}
