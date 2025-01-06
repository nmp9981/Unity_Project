using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

//애니메이터 변수 관리용
enum AnimatorVar
{
    isMoving,
    isHit,
    isDie,
    isAttack
}

public class EnemyInfo
{
    public SpriteRenderer spriteRenderer;
    public int HP;
    public int Exp;
    public int Attack;

    public EnemyInfo(SpriteRenderer spriteRenderer, int hP, int exp, int attack)
    {
        this.spriteRenderer = spriteRenderer;
        HP = hP;
        Exp = exp;
        Attack = attack;
    }
}
public class EnemyUnit : MonoBehaviour
{
    public int HP;
    public int Exp;
    public int Attack;
    public int Meso;

    Animator anim;

    float moveSpeed = 2f;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isDie", false);
        anim.SetBool("isHit", false);
    }
    private void OnEnable()
    {
        //EnemyInfo enemy = new EnemyInfo(dp,10,10,10);
        

    }
    private void Update()
    {
        MoveEnemy();
    }
    /// <summary>
    /// 기능 : 적 이동
    /// 성 넘어가면 오브젝트 비활성화
    /// </summary>
    void MoveEnemy()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        anim.Play("Move");
        //공격 대상에서 벗어남
        if (transform.position.x < -2f)
        {
            GameManager.Instance.ActiveUnitList.Remove(gameObject);
        }

        //비활성화
        if (transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            moveSpeed = 0;
        }
    }
}
