Shader "Custom/GPUMarchingCubes enderMesh"
{
    Properties
	{
		_SegmentNum("SegmentNum", int) = 32

		_Scale("Scale", float) = 1
		_Threashold("Threashold", float) = 0.5

		_DiffuseColor("Diffuse", Color) = (0,0,0,1)

		_EmissionIntensity("Emission Intensity", Range(0,1)) = 1
		_EmissionColor("Emission", Color) = (0,0,0,1)

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
	}

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

        #define UNITY_PASS_DEFERRED
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "UnityPBSLighting.cginc"

		#include "Libs/Primitives.cginc"
		#include "Libs/Utils.cginc"
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        //메시에서 넘어오는 정점 데이터
		struct appdata
		{
			float4 vertex	: POSITION;	//정점 좌표
		};

		//정점 쉐이더에서 지오메츠리 쉐이더를 반환
		struct v2g
		{
			float4 pos : SV_POSITION;	// 정점좌표
		};

		// 실제 렌더링시 제오메트리 -> 프래그먼트 쉐이더 반환
		struct g2f_light
		{
			float4 pos			: SV_POSITION;	//로컬 좌표
			float3 normal		: NORMAL;		//법선
			float4 worldPos		: TEXCOORD0;	//월드 좌표
			half3 sh : TEXCOORD3;				// SH
		};
		
		// 가상 렌더링시 제오메트리->프래그먼트
		struct g2f_shadow
		{
			float4 pos			: SV_POSITION;	// 좌표
			float4 hpos			: TEXCOORD1;
		};

		//변수 정의
		int _SegmentNum;

		float _Scale;
		float _Threashold;

		float4 _DiffuseColor;
		float3 _HalfSize;
		float4x4 _Matrix;

		float _EmissionIntensity;
		half3 _EmissionColor;

		half _Glossiness;
		half _Metallic;

		StructuredBuffer<float3> vertexOffset;
		StructuredBuffer<int> cubeEdgeFlags;
		StructuredBuffer<int2> edgeConnection;
		StructuredBuffer<float3> edgeDirection;
		StructuredBuffer<int> triangleConnectionTable;

		//정점 쉐이더
		v2g vert(appdata v){
			v2g o = (v2g)0;
			o.pos = v.vertex;
			return o;
		}

        ENDCG
    }
    FallBack "Diffuse"
}
