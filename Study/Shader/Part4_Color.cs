Shader"Custom/Simple"
{
    Properties
    {
        
        _Red("Red",Range(0,1)) = 0
        _Green("Green",Range(0,1)) = 0
        _Blue("Blue",Range(0,1)) = 0
        _Bright("Bright",Range(-1,1)) = 0//밝고 어둡게
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
       
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        struct Input
        {
            float2 uv_MainTex;
        };

        float _Red;
        float _Green;
        float _Blue;
        float _Bright;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            
            o.Albedo = float3(_Red, _Green, _Blue) +_Bright;
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
