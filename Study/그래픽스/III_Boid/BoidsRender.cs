using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GPUBoids))]
public class BoidsRender : MonoBehaviour
{
    #region 변수
    //그리기 할 Boids 객체의 스케일
    public Vector3 ObjectScale = new Vector3(0.1f, 0.2f, 0.5f);

    #region Reference Script
    public GPUBoids GPUBoidsScript;
    #endregion

    #region 리소스 생성
    //그릴 메시
    public Mesh instanceMesh;
    //그릴 머터리얼
    public Material instanceRendererMaterial;
    #endregion

    #region private 변수
    //GPU 인스턴싱을 위한 인수 (ComputerBuffer 전송용) 
    //인스턴스 당 인덱스 수, 인스턴스 수,
    //시작 인덱스 위치, 베이스 정점 위치, 인스턴스의 시작 위치
    uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    //GPU 인스턴싱을 위한 인수버퍼
    ComputeBuffer argsBuffer;
    #endregion private 변수
    #endregion 변수

    #region Unity 생명주기 함수
    void Start()
    {
        //인수 버퍼 초기화
        argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    void Update()
    {
        //메시 인스턴싱
        RenderInstancedMesh();
    }

    private void OnDisable()
    {
        //인수 버퍼 해제
        if (argsBuffer != null)
        {
            argsBuffer.Release();
        }
        argsBuffer = null;
    }
    #endregion

    #region private Function
    void RenderInstancedMesh()
    {
        //렌더링용 메터리얼이 Null 또는 GPUBoids 스크립트가 Null,
        //또는 GPU 인스턴싱이 지원되지 않으면 처리를 하지 않는다
        if (instanceRendererMaterial == null || GPUBoidsScript == null || !SystemInfo.supportsInstancing)
        {
            return;
        }

        //지정한 메시의 인덱스 가져오기
        uint numIndics = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        //메시의 인덱스 설정
        args[0] = numIndics;
        //인스턴스 수 초기화  :최대 객체 수
        args[1] = (uint)GPUBoidsScript.GetMaxObjectNum();
        argsBuffer.SetData(args);

        //Boid Data 저장 버퍼를 메테리얼에 설정
        instanceRendererMaterial.SetBuffer("_BoidDataBuffer", GPUBoidsScript.GetBoidDataBuffer());

        //Boid 객체 스타일 설정  :객체 영역
        instanceRendererMaterial.SetVector("_ObjectScale", ObjectScale);

        //bound 중심, 크기 정의
        var bounds = new Bounds(GPUBoidsScript.GetSimulationAreaCenter(),GPUBoidsScript.GetSimulationAreaSize());

        //메시를 GPU 인스턴싱하며 그리기
        //인스턴싱 메시, 인덱스, 그릴 메테리얼, 영역, 인수의 버퍼
        Graphics.DrawMeshInstancedIndirect(instanceMesh,0,instanceRendererMaterial,bounds,argsBuffer);
    }
    #endregion
}
