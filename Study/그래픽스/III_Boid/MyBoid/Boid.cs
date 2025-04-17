using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Boid : MonoBehaviour
{
    //각 Boid의 이웃들
    [Header("Neighbor")]
    List<GameObject> nearNeighbors = new List<GameObject>();

    //이동 방향 및 속도
    [Header("MoveInform")]
    [SerializeField] private Vector3 velocity;

    BoidSpawner spawner;

    //이웃 개수
    [Header("TEST")]
    [SerializeField] private int neighborCount = 0;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        spawner = BoidSpawner.Instance;
        velocity = transform.forward * spawner.maxSpeed;
    }

    void Update()
    {
        FindNeighbors();

        velocity += CalculateCohesion() * spawner.cohesionWeight;
        velocity += CalculateAlignment() * spawner.alignmentWeight;
        velocity += CalculateSeparation() * spawner.separationWeight;
        LimitMoveRadius();

        //최대속도 초과
        if (velocity.magnitude > spawner.maxSpeed) 
            velocity = velocity.normalized * spawner.maxSpeed;

        //최종 이동과 회전
        this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);
    }

    /// <summary>
    /// 근처 이웃찾기
    /// </summary>
    private void FindNeighbors()
    {
        nearNeighbors.Clear();

        //전체 오브젝트 탐색
        foreach(GameObject neighbor in spawner.Boids)
        {
            //이웃 수 제한 초과
            if (nearNeighbors.Count >= spawner.maxNeighbors)
            {
                return;
            }

            //자기 자신 제외
            if(neighbor == this.gameObject)
            {
                continue;
            }

            //두 물체간 거리
            float dist = Vector3.Distance(neighbor.transform.position, this.gameObject.transform.position);

            //범위내 이웃만 남기기
            if (dist < spawner.neighborDistance)
            {
                nearNeighbors.Add(neighbor);
            }
        }
        //이웃 개수
        neighborCount = nearNeighbors.Count;
    }

    /// <summary>
    /// 이웃들의 중간 위치로 가는 방향구하기
    /// </summary>
    /// <returns></returns>
    #region Cohesion 계산 메서드
    private Vector3 CalculateCohesion()
    {
        //응집 평균 방향
        Vector3 cohesionDirection = Vector3.zero;

        if (nearNeighbors.Count > 0)
        {
            for (int i = 0; i < nearNeighbors.Count; i++)
            {
                //방향차
                cohesionDirection += nearNeighbors[i].transform.position - this.transform.position;
            }
            //방향차의 평균
            cohesionDirection /= nearNeighbors.Count;
            cohesionDirection.Normalize();//정규화
        }
        return cohesionDirection;
    }
    #endregion

    /// <summary>
    /// 이웃들의 평균 방향 구하기
    /// </summary>
    /// <returns></returns>
    #region Alignment 계산 메서드
    private Vector3 CalculateAlignment()
    {
        Vector3 alignmentDirection = transform.forward;

        if (nearNeighbors.Count > 0)
        {
            for (int i = 0; i < nearNeighbors.Count; ++i)
            {
                //각 객체가 바라보는 방향
                alignmentDirection += nearNeighbors[i].transform.forward;
                //alignmentDirection += nearNeighbors[i].GetComponent<Boid>().velocity;
            }
            alignmentDirection /= nearNeighbors.Count;
            alignmentDirection.Normalize();
        }
        return alignmentDirection;
    }
    #endregion

    /// <summary>
    /// 이웃들한테서 벗어남
    /// </summary>
    /// <returns></returns>
    #region Separation 계산 메서드
    private Vector3 CalculateSeparation()
    {
        Vector3 separationDirection = Vector3.zero;

        if (nearNeighbors.Count > 0)
        {
            for (int i = 0; i < nearNeighbors.Count; ++i)
            {
                //이웃에서 벗어나는 방향
                separationDirection += (this.transform.position - nearNeighbors[i].transform.position);
            }
            separationDirection /= nearNeighbors.Count;
            separationDirection.Normalize();
        }
        return separationDirection;
    }
    #endregion

    /// <summary>
    /// 이동반경제한
    /// </summary>
    private void LimitMoveRadius()
    {
        //이동 반경이 더 작다면
        if (spawner.moveRadiusRange < this.transform.position.magnitude)
        {
            //원점-Boid 방향 : 벗어난 정도
            Vector3 diffDir = this.transform.position - Vector3.zero;
            // 원점->boid 방향 x -(boid가 벗어난 정도) x 힘 x 델타타임
            velocity += diffDir.normalized * (spawner.moveRadiusRange - diffDir.magnitude) * spawner.boundaryForce * Time.deltaTime;   
        }
    }
}
