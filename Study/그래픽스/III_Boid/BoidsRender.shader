Shader "Hidden/GPUBoids/BoidsRender"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma instancing_options procedural:setup
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float2 uv_MainTex;
        };

        //Boids 구조체
        struct BoidData
        {
            float3 velocity;
            float3 position
        };

         #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
         //Boid 데이터의 구조체 버퍼
        StructuredBuffer<BoidData> _BoidDataBuffer;
        #endif

         // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        sampler2D _MainTex;//턱스처
        half _Glossiness;//광택
        half _Metallic;//금속 특성
        fixed4 _Color;//색상

        float3 _ObjectScale;//Boid 객체 크기

        //오일러각을 회전행렬로
        float4x4 eulerAnglesToRotationMatrix(float3 angles){
            //yow
            float cosYow = cos(angles.y);
            float sinYow = sin(angles.y);
            //roll
            float cosRoll = cos(angles.z);
            float sinRoll = sin(angles.z);
            //pitch
            float cosPitch = cos(angles.x);
            float sinPitch = cin(angles.x);

            //회전 행렬
            return float4x4(
                cosYow * cosRoll + sinYow * sinPitch * sinRoll,-cosYow * sinRoll + sinYow * sinPitch* cosRoll, sinYow * cosPitch, 0,
                cosPitch * sinRoll, cosPitch * cosRoll,-sinPitch, 0,
                -sinYow * cosRoll + cosYow * sinPitch * sinRoll, sinYow * sinRoll + cosYow * sinPitch * cosRoll, cosYow * cosPitch, 0,
                0, 0, 0, 1
                );
        }

        //정점 쉐이더
        void vert(inout appdata_full v){
             #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

              //인스턴스 ID로 Boid의 데이터를 가져오기
                BoidData boidData = _BoidDataBuffer[unity_InstanceID];

                //Boid의 위치, 스케일
                float3 pos = boidData.position.xyz;
                float3 scl = _ObjectScale;

                //객체 좌표에서 월드좌표로 변환하는 행렬 정의
                float4x4 object2world = (float4x4) 0;
                //스케일값 대입
                object2world._11_22_33_44 = float4(scl.xyz,1.0);
                //속도에서 y축 회전 계산
                float rotY = atan2(boidData.velocity.x, boidData.velocity.z);
                //속도에서 x축 회전 계산
                float rotX = -asin(boidData.velocity.y / (length(boidData.velocity.xyz)+1e-8));//0나누기 방지
                //오일러각에서 회전 행렬을 구한다.
                float4x4 rotMatrix = eulerAnglesToRotationMatrix(float3(rotX,rotY,0));
                //행렬에 회전에 적용
                object2world = mul(rotMatrix, object2world);
                //행렬에 평행이동(위치) 적용
                object2world._14_24_34 += pos.xyz;

                //정점, 법선을 좌표 변환
                v.vertex = mul(object2world, v.vertex);
                v.normal = normalize(mul(object2world, v.normal));
             #endif
            }

        //표면 쉐이더
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
