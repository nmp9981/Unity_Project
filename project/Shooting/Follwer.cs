using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follwer : MonoBehaviour
{
    public float maxShotDelay;
    public float curShotDelay;
    public ObjectManager objectManager;

    public Vector3 followPos;//따라다닐 위치
    public int followDelay;//프레임 수
    public Transform parent;
    public Queue<Vector3> parentPos;//캐릭터 위치

    private void Awake()
    {
        parentPos = new Queue<Vector3>();
    }
    private void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }
    void Watch()
    {
        //선입선출, 위치 넣기
        //캐릭터의 위치들과 현재 캐릭터의 위치가 다를 경우만 위치를 넣는다.(가만히 있는 경우 제외)
        if(!parentPos.Contains(parent.position)) parentPos.Enqueue(parent.position);

        //일정 개수 채워지면 반환, 딜레이만큼 이전 프레임의 위치를 보조무기에 적용
        if(parentPos.Count > followDelay) followPos = parentPos.Dequeue();//플레이어의 위치 할당
        else if(parentPos.Count < followDelay) followPos = parent.position;//아직 다 안채워 졌으면 캐릭터의 위치
    }
    void Follow()
    {
        transform.position = followPos;//보조무기 위치  = 팔로워 위치
    }
    void Fire()
    {
        if (curShotDelay < maxShotDelay) return;//쿨타임이 안됨

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        bullet.transform.position = transform.position;
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        curShotDelay = 0;//쿨타임 초기화
    }
    //장전
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }
}
