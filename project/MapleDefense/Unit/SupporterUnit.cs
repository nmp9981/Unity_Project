using Cysharp.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SupporterUnit : MonoBehaviour
{
    [SerializeField]
    float unitSpeed;

    [SerializeField]
    float rayInspectDist;//ray 인식 거리

    //투사체 날리기
    [SerializeField]
    GameObject supporterBullet;
    [SerializeField]
    Transform startShootPos;

    /// <summary>
    /// 소환수 HP
    /// </summary>
    [SerializeField]
    float fullSupportHP;
    [SerializeField]
    public float currentSupportHP;

    [SerializeField]
    public int supportAttack;
    [SerializeField]
    float supportAttackSpeed = 0.5f;
    [SerializeField]
    Image hpBarBack;
    [SerializeField]
    Image hpBar;

    Animator anim;
    public bool isFighting;//전투중인가?

    float noHitDamageCurrentTime = 0f;
    float attackCurrentTime = 0f;

    const float maxInspectDist = 15;
    const float notHitDamageTime = 1;

    private void Awake()
    {
        InitAnimation();
    }

    private void OnEnable()
    {
        isFighting = false;
        currentSupportHP = fullSupportHP;
        hpBar.fillAmount = 1f;

        anim.Play("Move");
        StartCoroutine(SupporterAttack());
    }
    
    void Update()
    {
        MoveSupportUnit();
        TimeFlow();
    }
    /// <summary>
    /// 애니메이션 초기화
    /// </summary>
    void InitAnimation()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isDie", false);
        anim.SetBool("isAttack", false);
        anim.SetBool("isHit", false);
    }
    /// <summary>
    /// 시간 흐름
    /// </summary>
    void TimeFlow()
    {
        noHitDamageCurrentTime += Time.deltaTime;
        attackCurrentTime += Time.deltaTime;
    }

    /// <summary>
    /// 소환수 이동
    /// 전투중이, UI업그레이드 활성화가 아닐때만 이동
    /// HPBar도 이동
    /// </summary>
    void MoveSupportUnit()
    {
        if (!isFighting && !GameManager.Instance.IsOpenUpgradeUI)
        {
            transform.position += Vector3.right * Time.deltaTime * unitSpeed;
            HPBarMove();
        }
        else
        {
            transform.position += Vector3.right * Time.deltaTime * 0;
        }

        //소환수 수명 종료
        UnitDestroy();
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
    /// 적 인식
    /// </summary>
    void InspectEnemy()
    {
        foreach(var mob in GameManager.Instance.ActiveUnitList)
        {
            float dist = (mob.transform.position - gameObject.transform.position).sqrMagnitude;

            if(dist < rayInspectDist* rayInspectDist)
            {
                isFighting = true;
                return;
            }
        }
        isFighting = false;
    }
   
    /// <summary>
    /// 기능 : 적 공격
    /// </summary>
    IEnumerator SupporterAttack()
    {
        yield return new WaitForSeconds(1);//1초 지나고 공격
        while (true)
        {
            //적 인식
            InspectEnemy();
            //전투중일때
            if (isFighting)
            {
                //공격 쿨타임이 됨
                anim.SetBool("isAttack", true);

                //투사체 날리기(anim 실행시간)
                float readyTime = anim.GetCurrentAnimatorStateInfo(0).length;
                yield return new WaitForSeconds(readyTime);
                SupporterAttackThrowObject();

                yield return new WaitForSeconds(supportAttackSpeed * 0.2f);
                anim.SetBool("isAttack", false);
            }
            yield return new WaitForSeconds(supportAttackSpeed*0.3f);
        }
    }
    /// <summary>
    /// 투사체 날리기
    /// </summary>
    void SupporterAttackThrowObject()
    {
        //투사체가 없음
        if (supporterBullet == null)
        {
            return;
        }
        //투사체 발사 및 공격력 설정
        GameObject monsterBulletObj = Instantiate(supporterBullet);
        monsterBulletObj.transform.position = startShootPos.position;
        monsterBulletObj.GetComponent<SupporterThrowObjectClass>().supporterBasicAttack = supportAttack;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //피격
        if (collision.tag.Contains("Enemy"))
        {
            EnemyUnit enemyUnit = collision.gameObject.GetComponent<EnemyUnit>();
            //적 HP감소
            if (isFighting)
            {
                DecreaseEnemyHP(enemyUnit);
            }

            //소환수 HP감소
            int enemtHitDamage = enemyUnit.Attack;
            DecreaseSupportHP(enemtHitDamage);
        }
    }
    
    /// <summary>
    /// 적 HP감소
    /// </summary>
    void DecreaseEnemyHP(EnemyUnit enemyUnit)
    {
        if(attackCurrentTime >= supportAttackSpeed)
        {
            enemyUnit.DecreaseEnemyUnitHP(supportAttack);
            attackCurrentTime = 0;
        }
    }
    /// <summary>
    /// 소환수 HP감소
    /// </summary>
    public void DecreaseSupportHP(int enemtHitDamage)
    {
        if(noHitDamageCurrentTime >= notHitDamageTime)
        {
            currentSupportHP -= enemtHitDamage;
            float hpRate = (float)currentSupportHP / fullSupportHP;
            hpBar.fillAmount = hpRate;
            noHitDamageCurrentTime = 0;

            //사망 처리
            DieSupporter();
        }
    }
    /// <summary>
    /// 소환수 사망
    /// </summary>
    public void DieSupporter()
    {
        if (currentSupportHP <= 0)
        {
            GameManager.Instance.ActiveSupportUnitList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 적 성에 닿으면 사라짐
    /// </summary>
    void UnitDestroy()
    {
        if (gameObject.transform.position.x > 10)
        {
            gameObject.SetActive(false);
        }
    }
}
