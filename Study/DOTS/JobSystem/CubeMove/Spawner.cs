using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour//타겟, 감시자 스폰
{
    public static Transform[] TargetTransforms;//타겟의 위치를 담는 배열
    public static Transform[] SeekerTransforms;//목표물의 위치를 담는 배열

    public GameObject SeekerPrefab;//감시하는것
    public GameObject TargetPrefab;//타겟
    public int NumSeekers;
    public int NumTargets;
    public Vector2 Bounds;

    public void Awake()
    {
        Random.InitState(123);//랜덤 시드값

        SeekerTransforms = new Transform[NumSeekers];
        for(int i = 0; i < NumSeekers; i++)
        {
            GameObject go = GameObject.Instantiate(SeekerPrefab);
            Seeker seeker = go.GetComponent<Seeker>();
            Vector2 dir = Random.insideUnitCircle;// 반지름이 1인 원 안에서 위치를 Vector2로 랜덤으로 가져옴
            seeker.Direction = new Vector3(dir.x, 0, dir.y);//방향 설정
            SeekerTransforms[i] = go.transform;//배열에 위치 정보 넣기
            go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), 0, Random.Range(0,Bounds.y));
        }

        TargetTransforms = new Transform[NumTargets];
        for (int i = 0; i < NumTargets; i++)
        {
            GameObject go = GameObject.Instantiate(TargetPrefab);
            Target target = go.GetComponent<Target>();
            Vector2 dir = Random.insideUnitCircle;
            target.Direction = new Vector3(dir.x, 0, dir.y);//방향 설정
            TargetTransforms[i] = go.transform;
            go.transform.localPosition = new Vector3(Random.Range(0, Bounds.x), 0, Random.Range(0, Bounds.y));
        }
    }
}
