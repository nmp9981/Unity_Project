Shader "Custom/Fire"
{
    Properties
    {
        
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MainTex2 ("Albedo (RGB)", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        
        #pragma surface surf Standard alpha : fade

        sampler2D _MainTex;
        sampler2D _MainTex2;

        struct Input{
            float2 uv_MainTex;
            float2 uv_MainTex2;
        };

        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            /*
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 d = tex2D(_MainTex, float2(IN.uv_MainTex2.x,IN.uv_MainTex2.y-_Time.y));
            o.Emission = c.rgb*d.rgb;
            o.Alpha = c.a*d.a;
            */
            fixed4 d = tex2D(_MainTex, float2(IN.uv_MainTex2.x, IN.uv_MainTex2.y - _Time.y));
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex+d.r);
            o.Emission = c.rgb;
            o.Alpha = c.a;
}
        ENDCG
    }
    FallBack "Diffuse"
}
