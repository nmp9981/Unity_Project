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

		//구체의 중심에서의 거리, 결과가 음수면 좌표가 구체 내에 있다
		inline float sphere(float3 pos, float radius)
		{
			return length(pos)- radius;
		}
		
		//거리함수 결과 블렌드
		float smoothMax(float d1, float d2, float k)
		{
			float h = exp(k * d1) + exp(k * d2);
			return log(h) / k;
		}

		//샘플링 함수
		//볼륨 데이터에서 지정한 좌표의 스칼라 수치
		float Sample(float x,float y,float z){
			//좌표 그리드 범위
			if((x<=1)||(y<=1)||(z<=1)||(x>=(_SegmentNum-1)) || (y>=(_SegmentNum-1)) || (z>=(_SegmentNum-1))){
				return 0;
			}

			//단위 크기
			float3 size = float3(_SegmentNum, _SegmentNum, _SegmentNum);
			//단위 크기에서의 위치
			float3 pos = float3(x,y,z)/size;

			float3 spPos;
			float result = 0;

			//3개의 구의 거리함수 (거리 조건을 만족하는 함수)
			for(int i=0;i<3;i++){
				//pos : 격자의 각 꼭짓점, 구체 중심에서의 거리
				float sp = -sphere(pos - float3(0.5, 0.25 + 0.25 * i, 0.5), 0.005 + (sin(_Time.y * 8.0 + i * 23.365) * 0.5 + 0.5) * 0.125) 
				+ 0.5;
				result = smoothMax(result, sp, 14);
			}
			return result;
		}


		[maxvertexcount(15)]//최대 정점수, 1격자당 삼각폴리곤이 최대 5개
		void geom_light(point v2g input[1], inout TriangleStream<g2f_light> outStreame){

			g2f_light o = (g2f_light)0;
			int i, j;
			
			float cubeValue[8];//격자 8개 꼭짓점 스칼라 수치 배열

			//모서리 배열
			float3 edgeVertices[12] = {
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0) };

			//모서리 법선벡터
			float3 edgeNormals[12] = {
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0),
				float3(0, 0, 0) };

			float3 pos = input[0].pos.xyz;
			float3 defpos = pos;


			//스칼라 수치
			for(int i=0;i<8;i++){//i는 좌표 순번
				//오프셋 좌표값을 더한다
				cubeValue[i] = Sample(pos.x+vertexOffset[i].x, pos.y+vertexOffset[i].y, pos.z+vertexOffset[i].z);
			}

			pos *= _Scale;
			pos -= _HalfSize;

			int flagIndex = 0;

			//격자 8개 꼭짓점의 수치가 역치를 넘었는가?, 몇개가 넘었는가?
			for (i = 0; i < 8; i++) {
				if (cubeValue[i] <= _Threashold) {//역치 판단
					flagIndex |= (1 << i);
				}
			}

			int edgeFlags = cubeEdgeFlags[flagIndex];//비트로 폴리곤 생성정보 가져옴

			// 빈곳이면 아무것도 그리지 않는다
			if ((edgeFlags == 0) || (edgeFlags == 255)) {
				return;
			}

			//폴리곤의 정점 좌표 계산, 격자 주위상에 놓을 폴리곤 좌표 계산
			float offset = 0.5;
			float3 vertex;
			for (i = 0; i < 12; i++) {
				if ((edgeFlags & (1 << i)) != 0) {
					// 각 꼭짓점들의 역치 오프셋가져옴, 현재 모서리에서 다음 모서리까지의 비율(offset)
					offset = getOffset(cubeValue[edgeConnection[i].x], cubeValue[edgeConnection[i].y], _Threashold);

					//오프셋 기준으로 정점좌표 보정, 현재 모서리 + 다음 모서리 방향*비율
					vertex = (vertexOffset[edgeConnection[i].x] + offset * edgeDirection[i]);

					edgeVertices[i].x = pos.x + vertex.x * _Scale;
					edgeVertices[i].y = pos.y + vertex.y * _Scale;
					edgeVertices[i].z = pos.z + vertex.z * _Scale;

					// 법선 계산
					edgeNormals[i] = getNormal(defpos.x + vertex.x, defpos.y + vertex.y, defpos.z + vertex.z);
				}
			}

			//정점 좌표들을 연결하고 폴리곤 작성
			int vindex = 0;
			int findex = 0;
			// 最大５つの三角形ができる
			for (i = 0; i < 5; i++) {
				findex = flagIndex * 16 + 3 * i;
				if (triangleConnectionTable[findex] < 0)
					break;

				// 三角形を作る
				for (j = 0; j < 3; j++) {
					vindex = triangleConnectionTable[findex + j];

					// Transform行列を掛けてワールド座標に変換
					float4 ppos = mul(_Matrix, float4(edgeVertices[vindex], 1));
					o.pos = UnityObjectToClipPos(ppos);

					float3 norm = UnityObjectToWorldNormal(normalize(edgeNormals[vindex]));
					o.normal = normalize(mul(_Matrix, float4(norm,0)));

					outStream.Append(o);	// ストリップに頂点を追加
				}
				outStream.RestartStrip();	// 一旦区切って次のプリミティブストリップを開始
			}
		}

		//그림자 프래그먼트 쉐이더
		fixed4 frag_shadow(g2f_shadow i) : SV_Target
		{
			return i.hpos.z / i.hpos.w;
		}

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
