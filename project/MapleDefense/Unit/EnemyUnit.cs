using System.Collections;
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

    [SerializeField]
    GameObject monsterBullet;
    [SerializeField]
    Transform startShootPos;

    public int FullHP;
    public int HP;
    public uint Exp;
    public int Attack;
    public ulong Meso;
    public bool IsAttack;//마공 여부

    private int notAttackDamage;
    private int AttackDamage;

    Animator anim;

    float moveSpeed = 2f;

    //공격 관련 변수
    float attackCollTime;

    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        notAttackDamage = Attack;
        AttackDamage = (Attack * 3) / 2;
        anim.SetBool("isDie", false);
        anim.SetBool("isHit", false);
    }
    private void OnEnable()
    {
        //EnemyInfo enemy = new EnemyInfo(dp,10,10,10);
        HP = FullHP;
        hpBar.fillAmount = 1f;
        attackCollTime = Random.Range(250, 400)*0.01f;//쿨타임 설정
        anim.Play("Move");

        StartCoroutine(AttackEnemy());
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
        //업그레이드 UI가 활성화일때는 이동 금지
        if (GameManager.Instance.IsOpenUpgradeUI)
        {
            return;
        }

        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
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
        //HP Bar이동
        HPBarMove();
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
    IEnumerator AttackEnemy()
    {
        //마공안하는 몹
        if (!IsAttack)
        { 
            yield break;
        }

        yield return new WaitForSeconds(attackCollTime);
        int attackDamage = AttackDamage;
        int originDamage = notAttackDamage;
        while (true)
        {
            //공격 쿨타임이 됨
            //마공은 몸박의 1.5배
            anim.SetBool("isAttack", true);
            Attack = attackDamage;

            //투사체 날리기(anim 실행시간)
            yield return new WaitForSeconds(0.1f);
            float readyTime = anim.GetCurrentAnimatorStateInfo(0).length;
            yield return new WaitForSeconds(readyTime);
            MonsterAttackThrowObject(originDamage);

            yield return new WaitForSeconds(attackCollTime*0.5f);
            anim.SetBool("isAttack", false);
            Attack = originDamage;
            yield return new WaitForSeconds(attackCollTime);
        }
    }
    /// <summary>
    /// 기능 : 몬스터 사망 처리
    /// </summary>
    void DieMonster()
    {
        anim.SetBool("isDie", true);

        GameManager.Instance.CurrentMeso += Meso;
        GameManager.Instance.CurrentExp += Exp;
        GameManager.Instance.CastleLevelUP();
        GameManager.Instance.ActiveUnitList.Remove(gameObject);
        Invoke("EraseDieMonster", 0.5f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            //moveSpeed = 0;
            //피격 효과음
            SoundManager._sound.PlaySfx((int)SFXSound.CastleHit);
        }
        if (collision.tag.Contains("Bullet"))
        {
            //HP감소
            ThrowObject throwObj = collision.gameObject.GetComponent<ThrowObject>();
            if(throwObj != null)
            {
                int hitDamage = throwObj.Attack + throwObj.fromWeapon.weaponAttack;//무기+총알
                DecreaseEnemyUnitHP(hitDamage);
            }
        }
    }
    /// <summary>
    /// HP 감소
    /// </summary>
    /// <param name="hitDamage">피격 데미지</param>
    public void DecreaseEnemyUnitHP(int hitDamage)
    {
        HP -= hitDamage;
        float hpRate = (float)HP / FullHP;
        hpBar.fillAmount = hpRate;

        //공격 효과음
        SoundManager._sound.PlaySfx((int)SFXSound.MobHit);

        //피격 모션
        anim.SetBool("isHit", true);
        moveSpeed = 0;
        Invoke("ReturnMoveMotion", 0.5f);

        //사망처리
        if (HP <= 0)
        {
            DieMonster();
        }
    }

    void ReturnMoveMotion()
    {
        anim.SetBool("isHit", false);
        moveSpeed = 2;
    }
    /// <summary>
    /// 기능 : 몬스터 사라짐
    /// </summary>
    void EraseDieMonster()
    {
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 투사체 날리기
    /// </summary>
    void MonsterAttackThrowObject(int originDamage)
    {
        //투사체가 없음
        if (monsterBullet == null)
        {
            return;
        }
        //투사체 발사 및 공격력 설정
        GameObject monsterBulletObj = Instantiate(monsterBullet);
        monsterBulletObj.transform.position = startShootPos.position;
        monsterBulletObj.GetComponent<EnemyThrowObjectClass>().monsterBasicAttack = originDamage;
    }
}
