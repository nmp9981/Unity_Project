Shader "Unlit/fuck"
{
    Properties
    {
        _Color("Base Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        
        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            
            fixed4 _Color;

            //버텍스 구조체 선언
            struct vertexInput {
                float3 positionOnObjectSpace : POSITION;//변수 시멘틱
            };
            //프래그먼트 구조체
            struct fragmentInput {
                float4 positionOnClipSpace : SV_POSITION;
            };

            //클립공간에서의 위치로 변환
            fragmentInput vert(vertexInput input) {
                float4 positionOnClipSpace = UnityObjectToClipPos(input.positionOnObjectSpace);

                fragmentInput output;
                output.positionOnClipSpace = positionOnClipSpace;
                return output;
            }
            //색 채우기
            float4 frag(fragmentInput input) :SV_TARGET{
                return _Color;
            }
            ENDCG
        }
    }
}
