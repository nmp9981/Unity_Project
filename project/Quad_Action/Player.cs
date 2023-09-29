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

    //변수 생성
    public int ammo;
    public int coin;
    public int health;
    
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

    //장비 교체 키
    bool sDown1;
    bool sDown2;
    bool sDown3;

    bool isJump;
    bool isDodge;
    bool isSwap;

    Vector3 moveVec;
    Vector3 dodgeVec;//회피중 움직이지 않게

    Animator anim;
    Rigidbody rigid;

    GameObject nearObject;//감지된 아이템
    GameObject equipWeapon;//장착중인 아이템
    int equipWeaponIndex = -1;//장착중인 아이템 번호(처음부터 무기를 장착하면 안됨)

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //자식오브젝트의 컴포넌트 가져옴
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();//입력
        Move();//이동
        Turn();//회전
        Jump();//점프
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

        if (isSwap) moveVec = Vector3.zero;//무기 교체 중일때는 움직이지 않게
        
        transform.position += moveVec * speed * (wDown ? 0.3f : 1.0f) * Time.deltaTime;//좌표 이동

        //이동 애니메이션 적용하기
        anim.SetBool("isRun", moveVec != Vector3.zero);//멈춤만 아니면 기본 달리기
        anim.SetBool("isWalk", wDown);
    }
    //회전
    void Turn()
    {
        //회전하기
        //지정된 방향을 향해 회전, 우리가 갈 방향으로 회전
        transform.LookAt(this.transform.position + moveVec);
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
    //회피
    void Dodge()
    {
        //움직인 상태일때
        if (jDown && !isJump && moveVec!=Vector3.zero && !isDodge && !isSwap)//점프키 누르고 점프 , 무기 교체 상태가 아닐때
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

        //1~3키를 눌렀을때 교체(단, 점프중이거나 회피중이 아닐떄만)
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge)
        {
            if(equipWeapon!=null) equipWeapon.SetActive(false);//빈손이 아닐때만, 현재 장착중인 무기는 해제

            equipWeaponIndex = weaponIndex;//무기 변경
            equipWeapon = weapons[weaponIndex];//장작 무기
            equipWeapon.SetActive(true);

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
            }
        }
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
                    hasGrenades += item.value;
                    if (hasGrenades > maxHasGrenades) hasGrenades = maxHasGrenades;
                    break;
            }
            Destroy(other.gameObject);
        }
    }
    //아이템 감지
    private void OnTriggerStay(Collider other)
    {
        //무기
        if(other.tag == "Weapon")
        {
            nearObject = other.gameObject;

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;

        }
    }
}
