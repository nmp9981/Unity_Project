using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public bool isLook;//바라보는 중인가?

    Vector3 lookVec;//플레이어의 위치 추적용
    Vector3 tauntVec;//점프할 위치
    
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        nav.isStopped = true;//처음에는 추적을 멈춤

        StartCoroutine(Think());
    }

    // Update is called once per frame
    void Update()
    {
        //죽었으면
        if (isDead)
        {
            StopAllCoroutines();//모든 코루틴 멈춤
            return;
        }
        //바라보지 않을때만 추적
        if (isLook)
        {
            //플레이어 입력값으로 플레이어 위치 예측 벡터 생성
            float h = Input.GetAxisRaw("Horizon");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f;
            transform.LookAt(target.position + lookVec);//target은 플레이어
        }
        else nav.SetDestination(tauntVec);//점프 지점으로
    }
    //패턴 결정
    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        int ranAction = Random.Range(0, 5);//0~4
        switch(ranAction){
            case 0:
            case 1:
                StartCoroutine(MissileShot());
                break;
            case 2:
            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }
    IEnumerator MissileShot()
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;//타겟은 플레이어

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileA.target = target;//타겟은 플레이어

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());//끝나면 다음패턴으로

    }
    IEnumerator RockShot()
    {
        isLook = false;//기모을때는 바라보기 중지
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, transform.position, transform.rotation);//돌 생성
        
        yield return new WaitForSeconds(3f);//공격 끝
        isLook = true;

        StartCoroutine(Think());//끝나면 다음패턴으로
    }
    //점프후 내려찍기
    IEnumerator Taunt()
    {
        tauntVec = target.position + lookVec;//점프할 위치

        //점프시 목표지점으로 이동
        isLook = false;//점프시에는 바라보기 중지
        nav.isStopped = false;//추적 가능
        boxCollider.enabled = false;//플레이어를 밀면 안됨(비활성화)
        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);//내려찍기
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(1f);//공격 끝
        isLook = true;
        boxCollider.enabled = true;
        nav.isStopped = true;

        StartCoroutine(Think());//끝나면 다음패턴으로
    }
}
