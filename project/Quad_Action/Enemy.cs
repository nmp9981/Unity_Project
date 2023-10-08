using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C,D };//적의 타입을 나눔
    public Type enemyType;//적의 타입

    public int maxHealth;//체력
    public int curHealth;
    public Transform target;//타겟
    public BoxCollider meleeArea;//근접 공격 범위
    public GameObject bullet;//적 총알 프리팹

    public bool isChase;//추적 가능한가?
    public bool isAttack;//공격중인가?
    public bool isDead;//죽었는가?

    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] meshs;
    protected NavMeshAgent nav;//네비
    protected Animator anim;//자식에서도 쓸 수 있게 최소 protect

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        if(enemyType != Type.D) Invoke("ChaseStart",2f);//보스는 실행불가
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    private void Update()
    {
        if (nav.enabled && enemyType != Type.D)//네비가 활성화 일때만, 보스가 아닐떄
        {
            nav.SetDestination(target.position);//추적 가능할 때만 목표물 추척
            nav.isStopped = !isChase;//추적중이 아니면 멈춤
        }
        
    }
    //자동 이동 방지
    void FreezeVelocity()
    {
        if (isChase)//추적이 가능하면
        {
            rigid.velocity = Vector3.zero;//속도 0
            rigid.angularVelocity = Vector3.zero;//회전 속도를 0
        }
    }
    //범위 탐색
    void Targeting()
    {
        if (isDead) return;
        if (enemyType == Type.D) return;

        float targetRadius = 0f;//반지름
        float targetRange = 0f;//레이캐스트 길이

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;//정확도가 낮다
                targetRange = 3f;//감지범위가 좁다.
                break;
            case Type.B:
                targetRadius = 1f;
                targetRange = 12f;//감지범위가 넓다.
                break;
            case Type.C:
                targetRadius = 0.5f;//정확도가 높다
                targetRange = 25f;//감지범위가 넓다.
                break;
        }
        //원으로 raycast를 쏜다.(위치, 반지름, 방향(앞방향), 거리, 목표레이어)
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        
        //공격중이 아닌데, 플레이어가 감지됨
        if(rayHits.Length>0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }
    //공격
    IEnumerator Attack()
    {
        isChase = false;//추적중이 아님
        isAttack = true;//공격중
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A://일반
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;//공격범위 활성화

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;

                yield return new WaitForSeconds(1f);
                break;
            case Type.B://돌격
                yield return new WaitForSeconds(0.1f);
                //앞으로 돌격
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);

                meleeArea.enabled = true;//공격범위 활성화

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;//정지
                meleeArea.enabled = false;

                yield return new WaitForSeconds(2f);
                break;
            case Type.C://원거리
                yield return new WaitForSeconds(0.5f);
                GameObject instantBullet = Instantiate(bullet,transform.position,transform.rotation);//적 총알 생성(적이 있는 곳에)
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;//앞방향으로 속도를 줌

                yield return new WaitForSeconds(2f);
                break;
        }
        
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();//이동을 멈춤
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")//근접
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;

            Vector3 reactVec = transform.position - other.transform.position;//넉백
            StartCoroutine(OnDamage(reactVec,false));//피격 처리
        }else if(other.tag == "Bullet")//원격
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;//넉백
            Destroy(other.gameObject);//총알 삭제
            StartCoroutine(OnDamage(reactVec,false));//피격 처리
        }
    }
    //수류탄 맞음
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;//넉백
        StartCoroutine(OnDamage(reactVec, true));//피격 처리
    }

    //피격 처리
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        //전부 빨간색으로
        foreach(MeshRenderer mesh in meshs) mesh.material.color = Color.red;
       
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)//생성
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.white;
        }
        else//사망
        {
            foreach (MeshRenderer mesh in meshs) mesh.material.color = Color.gray;

            gameObject.layer = 14;//레이어 번호 변경(더이상 물리효과 못받게)
            isChase = false;
            nav.enabled = false;//네비 비활성화
            isDead = true;
            anim.SetTrigger("doDie");

            //넉백
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += (Vector3.up*3);

                rigid.freezeRotation = false;//회전이 가능하게 체크를 해제
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            if(enemyType != Type.D) Destroy(gameObject, 4f);
        }
    }
}
