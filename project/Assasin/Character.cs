using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [SerializeField] float CharacterSpeed = 0.2f;//조작 속도
    [SerializeField] public GameObject Dagger;//표창 객체
    [SerializeField] public Transform ThrowPosition;//쏘는 위치

    public float CoolTime;//쿨타임
    private float CulTime;//재시작까지 남은 시간

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Attack();
    }
    //캐릭터 이동
    void Move()
    {
        //캐릭터 이동
        float MoveAmount = CharacterSpeed * 1.5f;//캐릭터 이동 속도
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-MoveAmount, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveAmount, 0, 0);
        }
    }
    //캐릭터 공격
    void Attack()
    {
        if (CulTime <= 0)//CulTime이 0이하일 때만 k키가 눌리게함
        {
            if (Input.GetKey(KeyCode.K))
            {
                Instantiate(Dagger, ThrowPosition.position, ThrowPosition.rotation);//발사되는 위치 초기화
                ObjectPool.GetObject();//오브젝트 생성
            }                                                           
            CulTime = CoolTime;//다시 쿨타임으로 초기화
        }
        CulTime -= Time.deltaTime;//매 프레임마다 시간 감소
    }
}
