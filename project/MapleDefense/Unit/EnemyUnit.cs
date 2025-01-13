using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

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
    [SerializeField]
    Image hpBarBack;
    [SerializeField]
    Image hpBar;

    public int FullHP;
    int HP;
    public uint Exp;
    public int Attack;
    public ulong Meso;
    public bool IsAttack;//마공 여부

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
        HP = FullHP;
        hpBar.fillAmount = 1f;
    }
    private void Update()
    {
        MoveEnemy();
        HPBarMove();
        AttackEnemy();
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
        if (transform.position.x < -5f)
        {
            GameManager.Instance.ActiveUnitList.Remove(gameObject);
        }

        //비활성화
        if (transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 기능 : HPBar 이동
    /// </summary>
    void HPBarMove()
    {
        hpBarBack.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1, 0));
        hpBar.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1, 0));
    }
    /// <summary>
    /// 기능 : 적 공격
    /// </summary>
    void AttackEnemy()
    {
        //마공안하는 몹
        if (!IsAttack)
        {
            return;
        }
        // TODO : 마공 로직 추가, 마공은 몸박의 1.5배
    }
    /// <summary>
    /// 기능 : 몬스터 사망 처리
    /// </summary>
    void DieMonster()
    {
        GameManager.Instance.CurrentMeso += Meso;
        GameManager.Instance.CurrentExp += Exp;
        GameManager.Instance.CastleLevelUP();
        GameManager.Instance.ActiveUnitList.Remove(gameObject);
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            moveSpeed = 0;
        }
        if (collision.tag.Contains("Bullet"))
        {
            //HP감소
            ThrowObject throwObj = collision.gameObject.GetComponent<ThrowObject>();
            int hitDamage = throwObj.Attack + throwObj.fromWeapon.weaponAttack;//무기+총알
            HP -= hitDamage;
            float hpRate = (float)HP / FullHP;
            hpBar.fillAmount = hpRate;

            //사망처리
            if (HP <= 0)
            {
                DieMonster();
            }
        }
    }

}
