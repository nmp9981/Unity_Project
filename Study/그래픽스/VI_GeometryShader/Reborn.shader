Shader "Unlit/MyGeometry"
{
    Properties
    {
         _Height("Height", float) = 0.5
         _TopColor("Top Color", Color) = (1.0, 0.0, 1.0, 1.0)
         _BottomColor("Bottom Color", Color) = (0.0, 0.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 5.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            uniform float _Height;
            uniform float4 _TopColor, _BottomColor;

            struct v2g{
                float4 pos : SV_POSITION;
             };

            struct g2f{
                 float4 pos : SV_POSITION;
                 float4 col : COLOR;
            };

            v2g vert(appdata_full v){
                v2g o;
                o.pos = v.vertex;
                return o;
            }
            
            [maxvertexcount(8)]
             void geom(line v2g input[2],inout LineStream<g2f> outStream){
                float4 p0 = input[0].pos;
                float4 p1 = input[1].pos;
                float4 p2 = p0 + float4(0,_Height,0,1);
                float4 p3 = p1+float4(0,_Height,0,1);;
               
                g2f bottom0;
                bottom0.pos = UnityObjectToClipPos(p0);
                bottom0.col = _BottomColor;

                g2f bottom1;
                bottom1.pos = UnityObjectToClipPos(p1);
                bottom1.col = _BottomColor;

                g2f bottom2;
                bottom2.pos = UnityObjectToClipPos(p2);
                bottom2.col = _BottomColor;

                g2f bottom3;
                bottom3.pos = UnityObjectToClipPos(p3);
                bottom3.col = _BottomColor;
                
                //capsule

                outStream.Append(bottom1);
                outStream.Append(bottom2);
                outStream.RestartStrip();

                outStream.Append(bottom2);
                outStream.Append(bottom0);
                outStream.RestartStrip();

                outStream.Append(bottom0);
                outStream.Append(bottom3);
                outStream.RestartStrip();

                 outStream.Append(bottom1);
                outStream.Append(bottom3);
                outStream.RestartStrip();
             }

            float4 frag(g2f i) : SV_Target
            {
                return i.col;
            }
            ENDCG
        }
    }
}
