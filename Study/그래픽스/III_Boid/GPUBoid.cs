using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class GPUBoids : MonoBehaviour
{
    //Boids 데이터 구조체
    [System.Serializable]
    struct BoidData
    {
        public Vector3 Velocity;//속도
        public Vector3 Position;//위치
    }

    //스레드 그룹의 크기
    const int SIMULATION_GROUP_SIZE = 256;

    #region boids 관련 변수
    //최대 개체 수
    [Range(256, 32768)]
    public int MaxObjectNum = 16384;
    //결합을 적용하는 다른 개체와의 반경
    public float CohesionNeighborhoodRadius = 2.0f;
    //정렬을 적용하는 다른 개체와의 반경
    public float AlignmentNeighborhoodRadius = 2.0f;
    //분리를 적용하는 다른 개체와의 반경
    public float SeparateNeighborhoodRadius = 1.0f;
    //속도의 최대 값
    public float MaxSpeed = 5.0f;
    //조향력의 최대치
    public float MaxSteerForce = 0.5f;
    //결합하는 힘의 시뮬레이션 가중치
    public float CohesionWeight = 1.0f;
    //정렬하는 힘의 시뮬레이션 가중치
    public float AlignmentWeight = 1.0f;
    //분리하는 힘의 시뮬레이션 가중치
    public float SeparateWeight = 3.0f;
    //벽을 피하는 힘의 시뮬레이션 가중치
    public float AvoidWallWeight = 10.0f;
    //벽의 중심 좌표
    public Vector3 WallCenter = Vector3.zero;
    //벽의 크기
    public Vector3 WallSize = new Vector3(32.0f, 32.0f, 32.0f);
    #endregion

    #region Built-in Resources
    //Boids 시뮬레이션을 실행하는 ComputeShader의 참조
    public ComputeShader BoidsCS;
    #endregion
    
    #region Private Resources
    //Boid 조향력(Force)을 포함하는 버퍼
    ComputeBuffer _boidForceBuffer;
    //Boid의 기본 데이터(속도, 위치)를 포함하는 버퍼
    ComputeBuffer _boidDataBuffer;
    #endregion

    #region Access
    //Boids의 기본 데이터를 저장하는 버퍼를 반환
    public ComputeBuffer GetBoidDataBuffer()
    {
        if (this._boidDataBuffer == null)
        {
            return null;
        }
        return this._boidDataBuffer;
    }
    //최대 개체 수 반환
    public int GetMaxObjectNum()
    {
        return this.MaxObjectNum;
    }
    //시뮬레이션 영역의 중심 좌표 반환
    public Vector3 GetSimulationAreaCenter()
    {
        return this.WallCenter;
    }
    //시뮬레이션 영역의 박스 크기 반환
    public Vector3 GetSimulationAreaSize()
    {
        return this.WallSize;
    }
    #endregion

    #region Unity 생명 주기 함수
    private void Start()
    {
        //버퍼 초기화
        InitBuffer();
    }
    private void Update()
    {
        //시뮬레이션
        Simulation();
    }
    private void OnDestroy()
    {
        //버퍼 해제
        ReleaseBuffer();
    }
    private void OnDrawGizmos()
    {
        //디버그로서 시뮬레이션 영역을 와이어 프레임으로 렌더링
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(WallCenter, WallSize);
    }
    #endregion

    #region Private Function
    /// <summary>
    /// 버퍼 초기화
    /// </summary>
    void InitBuffer()
    {
        //버퍼 초기화 (버퍼 개수, 타입)\
        // ComputeBuffer는 ComputeShader을 위해 데이터를 저장하는 데이터 타입
        //Marshal.SizeOf( ) 메소드를 사용해 버퍼 요소로 사용할 자료형의 바이트 단위 크기
        _boidDataBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(BoidData)));//BoidData
        _boidForceBuffer = new ComputeBuffer(MaxObjectNum, Marshal.SizeOf(typeof(Vector3)));//조항력

        //버퍼 및 데이터 초기화
        //크기는 최대 오브젝트 개수 만큼
        var forceArr = new Vector3[MaxObjectNum];
        var boidDataArr = new BoidData[MaxObjectNum];
        for (int i = 0; i < MaxObjectNum; i++)
        {
            forceArr[i] = Vector3.zero;//조항력은 0
            //위치 속도 랜덤 지정
            //특정 반경 안의 랜덤 값 지정(Vector3에서 쓰임)
            boidDataArr[i].Position = Random.insideUnitSphere * 1.0f;
            boidDataArr[i].Velocity = Random.insideUnitSphere * 0.1f;
        }
        //버퍼 세팅, 구조체 배열값 세팅
        _boidDataBuffer.SetData(forceArr);
        _boidForceBuffer.SetData(boidDataArr);
        //배열 초기화
        forceArr = null;
        boidDataArr = null;
    }

    /// <summary>
    /// 군집 시뮬레이션
    /// </summary>
    void Simulation()
    {
        //Computer Shader를 가져옴
        ComputeShader cs = BoidsCS;

        //커널 ID
        int id = -1;
        //스레드그룹 수를 구하기
        int threadGroupSize = Mathf.CeilToInt(MaxObjectNum/ SIMULATION_GROUP_SIZE);
        //조향력을 계산
        id = cs.FindKernel("ForceCS"); //커널 ID를 가져옴
        
        //변숫값 설정
        cs.SetInt("_MaxBoidObjectNum", MaxObjectNum);
        cs.SetFloat("_CohesionNeighborhoodRadius",CohesionNeighborhoodRadius);
        cs.SetFloat("_AlignmentNeighborhoodRadius",AlignmentNeighborhoodRadius);
        cs.SetFloat("_SeparateNeighborhoodRadius",SeparateNeighborhoodRadius);
        cs.SetFloat("_MaxSpeed", MaxSpeed);
        cs.SetFloat("_MaxSteerForce", MaxSteerForce);
        cs.SetFloat("_SeparateWeight", SeparateWeight);
        cs.SetFloat("_CohesionWeight", CohesionWeight);
        cs.SetFloat("_AlignmentWeight", AlignmentWeight);
        cs.SetVector("_WallCenter", WallCenter);
        cs.SetVector("_WallSize", WallSize);
        cs.SetFloat("_AvoidWallWeight", AvoidWallWeight);
        //버퍼 설정
        cs.SetBuffer(id, "_BoidDataBufferRead", _boidDataBuffer);
        cs.SetBuffer(id, "_BoidForceBufferWrite", _boidForceBuffer);
        //ComputeShader를 실행
        //커널ID, 스레드 그룹 수 지정
        cs.Dispatch(id, threadGroupSize, 1, 1);

        //계산된 조항력으로부터 속도와 위치를 업데이트
        id = cs.FindKernel("IntegrateCS"); //커널 ID를 가져옴
        //변숫값 설정
        cs.SetFloat("_DeltaTime", Time.deltaTime);
        //버퍼 설정
        cs.SetBuffer(id, "_BoidForceBufferRead", _boidForceBuffer);
        cs.SetBuffer(id, "_BoidDataBufferWrite", _boidDataBuffer);
        //ComputeShader를 실행
        cs.Dispatch(id, threadGroupSize, 1, 1);
    }

    /// <summary>
    /// 버퍼 해제
    /// </summary>
    void ReleaseBuffer()
    {
        if (_boidDataBuffer != null)
        {
            _boidDataBuffer.Release();
            _boidDataBuffer = null;
        }
        if (_boidForceBuffer != null)
        {
            _boidForceBuffer.Release();
            _boidForceBuffer = null;
        }
    }
    #endregion
}
