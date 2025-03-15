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
        
        CGINCLUDE
        #define UNITY_PASS_DEFERRED
		#include "HLSLSupport.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "UnityPBSLighting.cginc"

		//Libs폴더에 해당 cg파일이 있어야함
		#include "Libs/Primitives.cginc"
		#include "Libs/Utils.cginc"

        //메시에서 넘어오는 정점 데이터
		struct appdata
		{
			float4 vertex : POSITION;	//정점 좌표
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
		float sphere(float3 pos, float radius)
		{
			return length(pos)- radius;
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
				float sp = -sphere(pos - float3(0.5, 0.25 + 0.25 * i, 0.5), 0.005 + (sin(_Time.y * 8.0 + i * 23.365) * 0.5 + 0.5) * 0.125) + 0.5;
				result = smoothMax(result, sp, 14);
			}
			return result;
		}

		//오프셋 계산
		float getOffset(float val1, float val2, float desired){
			float delta = val2-val1;
			if(delta==0.0){
				return 0.5;
			}
			return (desired-val1)/delta;
		}

		//노멀 계산
		float3 getNormal(float fX, float fY, float fZ){
			float3 normal;
			float offset = 1.0;

			normal.x = Sample(fX-offset,fY,fZ) - Sample(fX+offset, fY, fZ);
			normal.y = Sample(fX,fY-offset,fZ) - Sample(fX, fY+offset, fZ);
			normal.z = Sample(fX,fY,fZ-offset) - Sample(fX, fY, fZ+offset);

			return normal;
		}

		//정점
		v2g vert(appdata v){
			v2g o = (v2g)0;
			o.pos = v.vertex;
			return o;
		}


		//실제 지오메트리
		[maxvertexcount(15)]//최대 정점수, 1격자당 삼각폴리곤이 최대 5개
		void geom_light(point v2g input[1], inout TriangleStream<g2f_light> outStream){

			g2f_light o = (g2f_light)0;
			int i, j;
			
			float cubeValue[8];//격자 8개 꼭짓점 스칼라 수치 배열

			//정점 배열
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
			for(int idx=0;idx<8;idx++){//i는 좌표 순번
				//오프셋 좌표값을 더한다
				cubeValue[idx] = Sample(pos.x+vertexOffset[idx].x, pos.y+vertexOffset[idx].y, pos.z+vertexOffset[idx].z);
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
			// 최대 5개 삼각형을 만들 수 있다.
			for (i = 0; i < 5; i++) {
				findex = flagIndex * 16 + 3 * i;
				if (triangleConnectionTable[findex] < 0)
					break;

				//삼각형 제작
				for (j = 0; j < 3; j++) {
					vindex = triangleConnectionTable[findex + j];

					// Transform행렬 곱하여 월드좌표로 변환
					float4 ppos = mul(_Matrix, float4(edgeVertices[vindex], 1));
					//스크린 좌표로 변환
					o.pos = UnityObjectToClipPos(ppos);

					//법선 벡터를 월드 좌표로 변환 (프래그먼트 쉐이더에서 라이킹으로 사용)
					float3 norm = UnityObjectToWorldNormal(normalize(edgeNormals[vindex]));
					o.normal = normalize(mul(_Matrix, float4(norm,0)));

					outStream.Append(o);	//정점 추가
				}
				outStream.RestartStrip();	//일단 구분, 다음 primitive스트립
			}
		}

		//실제 fragment쉐이더
		//라이팅 부분 반영 -> surfaceShader의 라이팅 처리부분을 이식
		void frag_light(g2f_light IN,
			out half4 outDiffuse		: SV_Target0,
			out half4 outSpecSmoothness : SV_Target1,
			out half4 outNormal			: SV_Target2,
			out half4 outEmission		: SV_Target3)
		{
			fixed3 normal = IN.normal;

			float3 worldPos = IN.worldPos;

			fixed3 worldViewDir = normalize(UnityWorldSpaceViewDir(worldPos));

			 //SurfaceOutputStandard 구조체 초기화 (색, 광택 세팅)
#ifdef UNITY_COMPILER_HLSL
			SurfaceOutputStandard o = (SurfaceOutputStandard)0;
#else
			SurfaceOutputStandard o;
#endif
			o.Albedo = _DiffuseColor.rgb;
			o.Emission = _EmissionColor * _EmissionIntensity;
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
			o.Occlusion = 1.0;
			o.Normal = normal;

			//GI관계 처리
			// Setup lighting environment
			UnityGI gi;
			UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
			// gi.indirect.diffuse = 0;
			// gi.indirect.specular = 0;
			// gi.light.color = 0;
			// gi.light.dir = half3(0, 1, 0);
			// gi.light.ndotl = LambertTerm(o.Normal, gi.light.dir);

			// Call GI (lightmaps/SH/reflections) lighting function
			UnityGIInput giInput;
			UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
			giInput.light = gi.light;
			giInput.worldPos = worldPos;
			giInput.worldViewDir = worldViewDir;
			giInput.atten = 1.0;

			giInput.ambient = IN.sh;

			giInput.probeHDR[0] = unity_SpecCube0_HDR;
			giInput.probeHDR[1] = unity_SpecCube1_HDR;

#if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMin[0] = unity_SpecCube0_BoxMin; // .w holds lerp value for blending
#endif

#if UNITY_SPECCUBE_BOX_PROJECTION
			giInput.boxMax[0] = unity_SpecCube0_BoxMax;
			giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
			giInput.boxMax[1] = unity_SpecCube1_BoxMax;
			giInput.boxMin[1] = unity_SpecCube1_BoxMin;
			giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
#endif

			LightingStandard_GI(o, giInput, gi);

			//빛의 반사량 계산(Emission)
			outEmission = LightingStandard_Deferred(o, worldViewDir, gi, outDiffuse, outSpecSmoothness, outNormal);
			outDiffuse.a = 1.0;

			//HD의 경우에는 exp로 압축되는 부분를 끼워넣고 덮어씌움
#ifndef UNITY_HDR_ON
			outEmission.rgb = exp2(-outEmission.rgb);
#endif
		}


		//그림자 Geometry Shader
		[maxvertexcount(15)]
		void geom_shadow(point v2g input[1], inout TriangleStream<g2f_shadow> outStream)
		{
			g2f_shadow o = (g2f_shadow)0;

			int i, j;
			float cubeValue[8];
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
				float3(0, 0, 0) 
			};
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
				float3(0, 0, 0) 
			};

			float3 pos = input[0].pos.xyz;
			float3 defpos = pos;

			for (i = 0; i < 8; i++) {
				cubeValue[i] = Sample(
					pos.x + vertexOffset[i].x,
					pos.y + vertexOffset[i].y,
					pos.z + vertexOffset[i].z
				);
			}

			pos *= _Scale;
			pos -= _HalfSize;

			int flagIndex = 0;

			for (i = 0; i < 8; i++) {
				if (cubeValue[i] <= _Threashold) {
					flagIndex |= (1 << i);
				}
			}

			int edgeFlags = cubeEdgeFlags[flagIndex];

			if ((edgeFlags == 0) || (edgeFlags == 255)) {
				return;
			}

			float offset = 0.5;
			float3 vertex;
			for (i = 0; i < 12; i++) {
				if ((edgeFlags & (1 << i)) != 0) {
					offset = getOffset(cubeValue[edgeConnection[i].x], cubeValue[edgeConnection[i].y], _Threashold);

					vertex = (vertexOffset[edgeConnection[i].x] + offset * edgeDirection[i]);

					edgeVertices[i].x = pos.x + vertex.x * _Scale;
					edgeVertices[i].y = pos.y + vertex.y * _Scale;
					edgeVertices[i].z = pos.z + vertex.z * _Scale;

					edgeNormals[i] = getNormal(defpos.x + vertex.x, defpos.y + vertex.y, defpos.z + vertex.z);
				}
			}

			int vindex = 0;
			int findex = 0;
			for (i = 0; i < 5; i++) {
				findex = flagIndex * 16 + 3 * i;
				if (triangleConnectionTable[findex] < 0)
					break;

				for (j = 0; j < 3; j++) {
					vindex = triangleConnectionTable[findex + j];

					float4 ppos = mul(_Matrix, float4(edgeVertices[vindex], 1));

					float3 norm;
					norm = UnityObjectToWorldNormal(normalize(edgeNormals[vindex]));

					float4 lpos1 = mul(unity_WorldToObject, ppos);
					o.pos = UnityClipSpaceShadowCasterPos(lpos1, normalize(mul(_Matrix, float4(norm, 0))));
					o.pos = UnityApplyLinearShadowBias(o.pos);
					o.hpos = o.pos;

					outStream.Append(o);
				}
				outStream.RestartStrip();
			}
		}

		//그림자 프래그먼트 쉐이더
		fixed4 frag_shadow(g2f_shadow i) : SV_Target
		{
			return i.hpos.z / i.hpos.w;
		}
        ENDCG

		//실제 렌더링
		Pass{
			Tags{"LightMode" = "Deferred"}

			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom_light
			#pragma fragment frag_light
			#pragma exclude_renderers nomrt
			#pragma multi_compile_prepassfinal noshadow
			ENDCG
		}

		//그림자 렌더링
		Pass{
			Tags{"LightMode" = "ShadowCaster"}
			ZWrite On ZTest LEqual
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma geometry geom_shadow
			#pragma fragment frag_shadow
			#pragma multi_compile_shadowcaster
			ENDCG
		}
    }
    FallBack "Diffuse"
}
