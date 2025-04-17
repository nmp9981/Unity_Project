using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    public static BoidSpawner Instance;

    public GameObject prefab;//Boid 프리팹

    [Header("Init")]
    public float InstantiateRadius;//처음 Boids 생성 반경
    public int number;//총 생성 개수

    //전체 Boids
    public List<GameObject> Boids = new List<GameObject>();

    [Header("MoveManage")]
    public float cohesionWeight = 1.0f;//응집 가중치
    public float alignmentWeight = 1.0f;//정렬 가중치
    public float separationWeight = 1.0f;//분리 가중치
    public float moveRadiusRange = 5.0f;    // 활동 범위 반지름
    public float boundaryForce = 3.5f;      // 범위 내로 돌아가게 하는 힘
    public float maxSpeed = 2.0f;//최대 속력
    public float neighborDistance = 3.0f;   // 이웃 탐색 범위
    public float maxNeighbors = 50;         // 이웃 탐색 수 제한

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < number; ++i)
        {
            //반경 InstantiateRadius내에 물체 생성
            Boids.Add(Instantiate(prefab, this.transform.position + Random.insideUnitSphere * InstantiateRadius, Random.rotation));
        }
    }
}
