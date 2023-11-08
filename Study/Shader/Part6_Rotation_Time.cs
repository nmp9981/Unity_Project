Shader"Custom/StandardShader"
{
    Properties
    {
       
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _FlowSpeed("Flow Speed",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200        

        CGPROGRAM
        #pragma surface surf Standard
        
        sampler2D _MainTex;
        float _FlowSpeed;
    
        struct Input
        {
            float2 uv_MainTex;
        };

        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, float2(IN.uv_MainTex.x + _Time.y*_FlowSpeed, IN.uv_MainTex.y));
            o.Emission = c.rgb;
    
            o.Alpha = c.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
