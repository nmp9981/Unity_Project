Shader "Custom/Glass"
{
    Properties
    {
       // 풀의 높이, 폭
    _Height("Height", float) = 25
    _Width("Width", float) = 0.5
    
    // 풀의 밑, 중간, 윗 부분 높이
    _BottomHeight("Bottom Height", float) = 0.3
    _MiddleHeight("Middle Height", float) = 0.4
    _TopHeight("Top Height", float) = 0.5
    
    // 풀의 밑, 중간, 윗 부분 폭
    _BottomWidth("Bottom Width", float) = 0.5
    _MiddleWidth("Middle Width", float) = 0.4
    _TopWidth("Top Width", float) = 0.2
    
    // 풀의 밑,중간,윗 부분 꺽인 정도
    _BottomBend("Bottom Bend", float) = 1.0
    _MiddleBend("Middle Bend", float) = 2.0
    _TopBend("Top Bend", float) = 3.0
    
    // 바람의 세기
    _WindPower("Wind Power", float) = 0.75
    
    // 풀 윗,아랫 부분 색
    _TopColor("Top Color", Color) = (0.0, 1.0, 0.0, 1.0)
    _BottomColor("Bottom Color", Color) = (0.0, 0.2, 0.3, 1.0)
    
    // 풀 높이의 랜덤성을 주기 위한 Noise 텍스처
    _HeightMap("Height Map", 2D) = "white"
    
    // 풀의 방향에 랜덤성을 주기 위한 Noise 텍스처
    _RotationMap("Rotation Map", 2D) = "black"
    
    // 바람 세기에 랜덤성을 주기 위한 Noise 텍스처
    _WindMap("Wind Map", 2D) = "black"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        LOD 100
        Cull Off
       
        Pass{
            CGPROGRAM
        #pragma target 5.0
        #include "UnityCG.cginc"
        #pragma vertex vert
        #pragma geometry geom
        #pragma fragment frag

        float _Height, _Width;
        float _BottomHeight, _MiddleHeight, _TopHeight;
        float _BottomWidth, _MiddleWidth, _TopWidth;
        float _BottomBend, _MiddleBend, _TopBend;
        float _WindPower;
        float4 _TopColor, _BottomColor;
        sampler2D _HeightMap, _RotationMap, _WindMap;

        struct v2g
        {
            float4 pos : SV_POSITION;
            float3 nor : NORMAL;
            float4 hei : TEXCOORD0;
            float4 rot : TEXCOORD1;
            float4 wind : TEXCOORD2;
        };
        struct g2f
        {
            float4 pos : SV_POSITION;
            float4 color : COLOR;
        };

        v2g vert(appdata_full v)
        {
            v2g o;
            float4 uv = float4(v.texcoord.xy, 0.0f, 0.0f);
            o.pos = v.vertex;
            o.nor = v.normal;
            o.hei = tex2Dlod(_HeightMap, uv);
            o.rot = tex2Dlod(_RotationMap, uv);
            o.wind = tex2Dlod(_WindMap, uv);
            return o;
        }

        [maxvertexcount(7)]
        void geom(triangle v2g i[3], inout TriangleStream<g2f> stream)
        {
            float4 p0 = i[0].pos;
            float4 p1 = i[1].pos;
            float4 p2 = i[2].pos;

            float3 n0 = i[0].nor;
            float3 n1 = i[1].nor;
            float3 n2 = i[2].nor;
 
            //풀, 높이 ,방향
            float height = (i[0].hei.r + i[1].hei.r + i[2].hei.r) / 3.0f;
            float rot = (i[0].rot.r + i[1].rot.r + i[2].rot.r) / 3.0f;
            float wind = (i[0].wind.r + i[1].wind.r + i[2].wind.r) / 3.0f;

            //풀의 중심부, 풀의 방향
            float4 center = ((p0 + p1 + p2) / 3.0f);
            float4 normal = float4(((n0 + n1 + n2) / 3.0f).xyz, 1.0f);
 
            //높이 계산
            float bottomHeight = height * _Height * _BottomHeight;
            float middleHeight = height * _Height * _MiddleHeight;
            float topHeight = height * _Height * _TopHeight;

            //너비 계산
            float bottomWidth = _Width * _BottomWidth;
            float middleWidth = _Width * _MiddleWidth;
            float topWidth = _Width * _TopWidth;
            
            rot = rot- 0.5f;
            float4 dir = float4(normalize((p2- p0) * rot).xyz, 1.0f);
            
            g2f o[7];
            // Bottom.
            o[0].pos = center- dir * bottomWidth;
            o[0].color = _BottomColor;
            o[1].pos = center + dir * bottomWidth;
            o[1].color = _BottomColor;
            // Bottom to Middle.
            o[2].pos = center- dir * middleWidth + normal * bottomHeight;
            o[2].color = lerp(_BottomColor, _TopColor, 0.33333f);
            o[3].pos = center + dir * middleWidth + normal * bottomHeight;
            o[3].color = lerp(_BottomColor, _TopColor, 0.33333f);
            // Middle to Top.
            o[4].pos = o[3].pos- dir * topWidth + normal * middleHeight;
            o[4].color = lerp(_BottomColor, _TopColor, 0.66666f);
            o[5].pos = o[3].pos + dir * topWidth + normal * middleHeight;
            o[5].color = lerp(_BottomColor, _TopColor, 0.66666f);
            // Top.
            o[6].pos = o[5].pos + dir * topWidth + normal * topHeight;
            o[6].color = _TopColor;
            // Bend.
            dir = float4(1.0f, 0.0f, 0.0f, 1.0f);
            o[2].pos += dir * (_WindPower * wind * _BottomBend) * sin(_Time);
            o[3].pos += dir * sin(_Time);
            o[4].pos += dir * (_WindPower * wind * _MiddleBend) * sin(_Time);
            o[5].pos += dir * (_WindPower * wind * _MiddleBend) * sin(_Time);
            o[6].pos += dir * (_WindPower * wind * _TopBend)* sin(_Time); 
            
            //7개 정점 등록
            [unroll]
            for (int i = 0; i < 7; i++) {
                o[i].pos = UnityObjectToClipPos(o[i].pos);
                stream.Append(o[i]);
            }
        }
        
        
        float4 frag(g2f i) : COLOR
        {
            return i.color;
        }
        ENDCG
      }   
   }
    FallBack "Diffuse"
}
