Shader"Custom/toon"
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

        cull front//뒷면이 보이게

        CGPROGRAM
        #pragma surface surf Nolight vertex:vert noshadow noambient
        #pragma target 3.0
        
void vert(inout appdata_full v)//버텍스 셰이더
{
    v.vertex.xyz = v.vertex.xyz + v.normal.xyz * 0.01;
}

struct Input
{
    float4 color : COLOR;
};
void surf(Input IN, inout SurfaceOutput o)
{
}
float4 LightingNolight(SurfaceOutput s, float3 lightDir, float atten)
{
    return float4(0, 0, 0, 1);
}
ENDCG

cull back//앞면이 보이게

CGPROGRAM
        #pragma surface surf Toon noambient
        sampler2D _MainTex;
sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        float4 LightingToon(SurfaceOutput s, float3 lightDir,float3 viewDir, float atten)
        {
            float ndotl = dot(s.Normal, lightDir) * 0.5 + 0.5;
    
    if (ndotl > 0.7)
        ndotl = 1;
    else if(ndotl > 0.4)
        ndotl = 0.3;
    else
        ndotl = 0;
    
    //Fresnel 연산
    float rim = abs(dot(s.Normal, viewDir));
    if (rim > 0.3)
        rim = 1;
    else
        rim = -1;
    
    float4 final;
    final.rgb = s.Albedo * ndotl * _LightColor0.rgb*rim;
    final.a = s.Alpha;
            return final;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
