Shader "Custom/tex"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {} 
        _BlackWhite("blackwhite",Range(-1,1)) = 0
        _MainTex2 ("Albedo (RGB)", 2D) = "white" {} 
        _LerpTest("LerpTest",Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
       
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _MainTex2;
        float _BlackWhite;
        float _LerpTest;

        struct Input
        {
            float2 uv_MainTex;
            float uv_MainTex2;
};

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
           
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);//텍스처 연산
            fixed4 d = tex2D(_MainTex2, IN.uv_MainTex); //텍스처 연산
            //o.Albedo = (c.r+c.g+c.b)/3+_BlackWhite;//흑백 조절
            //o.Albedo = lerp(c.rgb, d.rgb, _LerpTest);//선형 보간
            o.Albedo = lerp(c.rgb, d.rgb, 1-c.a); //선형 보간
            o.Alpha = c.a;
}
        ENDCG
    }
    FallBack "Diffuse"
}
