using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public GameObject[] weapons;
    public bool[] hasWeapons;//장착 무기
    public GameObject[] grenades;//공전무기
    public int hasGrenades;
    public GameObject granadeObject;
    public Camera followCamera;//마우스 회전용 카메라(메인 카메라)
    public GameObject floor;//바닥

    //변수 생성
    public int ammo;//플레이어가 소지하고 있는 총알 개수
    public int coin;
    public int health;
    public int score;

    //각 수치의 최댓값
    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    float hAxis;
    float vAxis;

    bool wDown;
    bool jDown;
    bool iDown;
    bool fDown;
    bool gDown;//수류탄
    bool rDown;

    //장비 교체 키
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;//장전 중인가?
    bool isFireReady = true;
    bool isBorder;//경계선에 닿았는가?
    bool isDamage;//데이지를 입었는가?
    bool isShop;//쇼핑 중인가?

    Vector3 moveVec;
    Vector3 dodgeVec;//회피중 움직이지 않게

    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs;//플레이어 메테리얼(몸,다리,팔 등)

    GameObject nearObject;//감지된 아이템
    public Weapon equipWeapon;//장착중인 아이템
    int equipWeaponIndex = -1;//장착중인 아이템 번호(처음부터 무기를 장착하면 안됨)
    float fireDelay;//공격 딜레이

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //자식오브젝트의 컴포넌트 가져옴
        anim = GetComponentInChildren<Animator>();
        //원래 GetComponent는 1개이지만 s를 붙여주면 여러개를 가져올 수 있다.
        meshs = GetComponentsInChildren<MeshRenderer>();//모든 자식 컴포넌트를 가져옴

        //무기 장착할때는 빛, 파티클 이펙트 꺼야함
        for (int i = 0; i < 3; i++)
        {
            weapons[i].GetComponentInChildren<Light>().gameObject.SetActive(false);
            weapons[i].GetComponentInChildren<ParticleSystem>().gameObject.SetActive(false);
        }

        PlayerPrefs.SetInt("MaxScore",1012);

    }

    // Update is called once per frame
    void Update()
    {
        GetInput();//입력
        Move();//이동
        Turn();//회전
        Jump();//점프
        Grenade();//수류탄
        Attack();//공격
        Reload();//장전
        Dodge();//회피
        Interation();//아이템 습득
        SwapWeapon();//무기교체
    }

    //입력
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");//단 1회 입력
        fDown = Input.GetButton("Fire1");//키 다운
        gDown = Input.GetButtonDown("Fire2");//수류탄 투척
        rDown = Input.GetButtonDown("Reload");//장전
        iDown = Input.GetButtonDown("Interation");//e키
        //장비교체(1~3키)
        sDown1 = Input.GetButtonDown("Swap1");//1
        sDown2 = Input.GetButtonDown("Swap2");//2
        sDown3 = Input.GetButtonDown("Swap3");//3
    }
    //이동
    private void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)
        
        if (isDodge) moveVec = dodgeVec;//회피중일때는 회피방향으로

        //교체중 or 장전중 or 공격중이면 움직이지 않게
        if (isSwap || isReload || !isFireReady) moveVec = Vector3.zero;//무기 교체 중일때는 움직이지 않게
        
        //벽과 충돌하지 않았을 경우에만 이동
        if(!isBorder) transform.position += moveVec * speed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;//좌표 이동

        //이동 애니메이션 적용하기
        anim.SetBool("isRun", moveVec != Vector3.zero);//멈춤만 아니면 기본 달리기
        anim.SetBool("isWalk", wDown);
    }
    //회전
    void Turn()
    {
        // 키보드에 의해 회전하기
        //지정된 방향을 향해 회전, 우리가 갈 방향으로 회전
        transform.LookAt(this.transform.position + moveVec);

        //마우스에 의해 회전
        if (fDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//스크린 월드로 레이를 쏜다.
            RaycastHit rayHit;//레이의 정보 저장
            
            if (Physics.Raycast(ray, out rayHit, 100))//ray맞은 위치의 정보를 저장
            {
                Vector3 nextVec = rayHit.point - transform.position;//레이가 닿았던 위치 - 내 위치 = 내 위치와 마우스 클릭 지점간 거리(방향)
                nextVec.y = 0;//높이는 무시해야함
                transform.LookAt(this.transform.position + nextVec);//플레이어가 바라보는 방향
            }
        }
    }
    //점프
    void Jump()
    {
        //멈춤상태일때 점프
        if (jDown && !isJump && moveVec==Vector3.zero && !isDodge && !isSwap)//점프키 누르고 점프, 무기 교체 상태가 아닐때
        {
            rigid.AddForce(Vector3.up * 25, ForceMode.Impulse);//즉시점프
            anim.SetBool("isJump", true);//점프
            anim.SetTrigger("doJump");//착지
            isJump = true;
        }
    }
    //수류탄
    void Grenade()
    {
        if (hasGrenades == 0) return;//남은 수류탄이 없음

        //장전중, 교체중일때는 수류탄 사용 불가
        if(gDown && !isReload && !isSwap)
        {
            //마우스를 클릭한 위치에 수류탄을 놓는다.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//스크린 월드로 레이를 쏜다.
            RaycastHit rayHit;//레이의 정보 저장

            if (Physics.Raycast(ray, out rayHit, 100))//ray맞은 위치의 정보를 저장(마우스로 클릭한 위치)
            {
                Vector3 nextVec = rayHit.point - transform.position;//레이가 닿았던 위치 - 내 위치 = 내 위치와 마우스 클릭 지점간 거리(방향)
                nextVec.y = 10;//위로 던짐(포물선)

                GameObject instantGrenade = Instantiate(granadeObject,transform.position,transform.rotation);//수류탄 생성
                Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
                rigidGrenade.AddForce(nextVec,ForceMode.Impulse);//해당 방향으로 던진다.
                rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);//회전까지

                hasGrenades--;
                grenades[hasGrenades].SetActive(false);//사용했으면 비활성화
            }
        }
    }
    //플레이어와 충돌시
    private void OnCollisionEnter(Collision collision)
    {
        //바닥에 닿았을 떄
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("isJump", false);//점프 완료
            isJump = false;
        }
    }
    //공격
    void Attack()
    {
        if (equipWeapon == null) return;//공격 하려면 무기는 장착해야함

        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;//공속(공격 가능)

        //무기 발동조건(우클릭+쿨타임+회피x+교체x+쇼핑x)
        if(fDown && isFireReady && !isDodge && !isSwap && !isShop)
        {
            equipWeapon.Use();//공격
            //근접 무기인가? 원격 무기인가에 따라 애니메이션이 달라짐
            anim.SetTrigger(equipWeapon.type==Weapon.Type.Melee? "doSwing":"doShot");//모션 실행
            fireDelay = 0;
        }
    }
    //장전
    void Reload()
    {
        if (equipWeapon == null) return;//무기 미장착
        if (equipWeapon.type == Weapon.Type.Melee) return;//근접 무기
        if (ammo == 0) return;//총알이 있어야함
        
        //장전 가능 상황(공격이 가능해야함, 장전, 쇼핑 중이 아니어야함)
        if(rDown && !isJump && !isDodge && !isSwap && isFireReady && !isReload && !isShop)
        {
            anim.SetTrigger("doReload");
            isReload = true;

            Invoke("ReloadOut", 1.5f);
        }
    }
    //장전상태 종료(장전 완료)
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;//최대 갯수보다 가지고 있는 총알의 개수가 적을 수도 있다. 
        equipWeapon.curAmmo = reAmmo;//최대 개수
        ammo -= reAmmo;
        isReload = false;
    }
    //회피
    void Dodge()
    {
        //움직인 상태일때
        if (jDown && !isJump && moveVec!=Vector3.zero && !isDodge && !isSwap && !isShop)//점프키 누르고 점프 , 무기 교체, 상점 상태가 아닐때
        {
            dodgeVec = moveVec;
            speed *= 2;//회피는 이동속도가 2배
            anim.SetTrigger("doDodge");
            isDodge = true;

            //회피중 점프 불가
            Invoke("DodgeOut",0.5f);//회피 상태 종료
        }
    }
    //회피상태 종료
    void DodgeOut()
    {
        speed *= 0.5f;//원래 속도
        isDodge = false;
    }
    //무기를 다 바꿈
    void SwapOut()
    {
        isSwap = false;
    }

    //무기 교체
    void SwapWeapon()
    {
        //이때는 무기교체 불가
        if (sDown1 && (!hasWeapons[0] || equipWeaponIndex == 0)) return;//무기가 없거나 중복 무기
        if (sDown2 && (!hasWeapons[1] || equipWeaponIndex == 1)) return;
        if (sDown3 && (!hasWeapons[2] || equipWeaponIndex == 2)) return;

        //1~3을 누르면 그에 맞는 인덱스 생성
        int weaponIndex = -1;
        if (sDown1) weaponIndex = 0;
        if (sDown2) weaponIndex = 1;
        if (sDown3) weaponIndex = 2;

        //1~3키를 눌렀을때 교체(단, 점프중이거나 회피중, 상점중이 아닐떄만)
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop)
        {
            if(equipWeapon!=null) equipWeapon.gameObject.SetActive(false);//빈손이 아닐때만, 현재 장착중인 무기는 해제

            equipWeaponIndex = weaponIndex;//무기 변경
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();//장작 무기
            equipWeapon.gameObject.SetActive(true);
           
            anim.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.4f);
        }
    }
    //아이템 습득
    void Interation()
    {
        //키를누르고 근처 오브젝트가 있고 점프중이 아닐떄
        if (iDown && nearObject!=null && !isJump)
        {
            //무기
            if (nearObject.tag == "Weapon")
            {
                //아이템 정보를 가져온다.
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;
                
                Destroy(nearObject);//먹었으면 사라짐
            }else if (nearObject.tag == "Shop")//상점에 가면 상점이 열림
            {
                //샵 정보를 가져온다.
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }
    //자동회전 방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;//회전 속도를 0
    }
    //벽 관통 방지
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 4, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 4,LayerMask.GetMask("Wall"));//벽과 충돌
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }
    //아이템 입수
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo+=item.value;
                    if (ammo > maxAmmo) ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > maxCoin) coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > maxHealth) health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    grenades[hasGrenades].SetActive(true);//먹으면 활성화
                    grenades[hasGrenades].gameObject.GetComponentInChildren<ParticleSystem>().Stop();//파티클 이펙트는 꺼야함
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);//아이템 삭제
        }else if (other.tag == "EnemyBullet")//적 총알(미사일)
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossAttack = other.name == "Boss Melee Area"?true:false;//보스 근접공격 여부
                StartCoroutine(OnDamage(isBossAttack));
            }
            //미사일이 사라져야함(맞으면 사라짐)
            if (other.GetComponent<Rigidbody>() != null) Destroy(other.gameObject);
        }
    }
    //플레이어 피격
    IEnumerator OnDamage(bool isBossAttack)
    {
        isDamage = true;
        //노란색으로 변환
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.yellow;
        }

        //보스 공격이면 넉백
        if (isBossAttack) rigid.AddForce(transform.forward * -25, ForceMode.Impulse);

        yield return new WaitForSeconds(1f);//무적 시간

        isDamage = false;
        //원래대로 변환
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
        if (isBossAttack) rigid.velocity = Vector3.zero;//원래대로
    }
    //아이템 감지
    private void OnTriggerStay(Collider other)
    {
        //무기와 상점
        if(other.tag == "Weapon" || other.tag=="Shop")
        {
            nearObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon") nearObject = null;
        else if (other.tag == "Shop")
        {
            //샵 정보를 가져온다.
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
}
