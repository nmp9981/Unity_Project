using UnityEngine;
using UnityEngine.UI;

public class SupporterUnit : MonoBehaviour
{
    [SerializeField]
    float unitSpeed;

    [SerializeField]
    float rayInspectDist;//ray 인식 거리

    /// <summary>
    /// 소환수 HP
    /// </summary>
    [SerializeField]
    float fullSupportHP;
    [SerializeField]
    float currentSupportHP;
    [SerializeField]
    int supportAttack;
    [SerializeField]
    Image hpBarBack;
    [SerializeField]
    Image hpBar;



    const float maxInspectDist = 15;

    private void OnEnable()
    {
        currentSupportHP = fullSupportHP;
        hpBar.fillAmount = 1f;
    }
    
    void Update()
    {
        MoveSupportUnit();
        InspectEnemy();
        SupporterAttack();
    }
    /// <summary>
    /// 소환수 이동
    /// 전투중이, UI업그레이드 활성화가 아닐때만 이동
    /// HPBar도 이동
    /// </summary>
    void MoveSupportUnit()
    {
        if (!GameManager.Instance.IsFighting && !GameManager.Instance.IsOpenUpgradeUI)
        {
            transform.position += Vector3.right * Time.deltaTime * unitSpeed;
            HPBarMove();
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
    /// 적 인식
    /// </summary>
    void InspectEnemy()
    {
        //RayCast로 물체 인식
        RaycastHit2D rayHitObj = Physics2D.Raycast(transform.position, transform.right, rayInspectDist);
        Debug.DrawRay(transform.position, transform.right * rayInspectDist, Color.red, 10);
        if (rayHitObj)
        {
            //적일 경우
            if (rayHitObj.collider.gameObject.CompareTag("Enemy"))
            {
                //Debug.Log("몬스터 인식");
                GameManager.Instance.IsFighting = true;
            }
        }
        else
        {
            GameManager.Instance.IsFighting = false;
        }
    }
    /// <summary>
    /// 기능 : 공격
    /// </summary>
    void SupporterAttack()
    {
        if (!GameManager.Instance.IsFighting)
        {
            return;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Enemy"))
        {
            //HP감소
            EnemyUnit enemyUnit = collision.gameObject.GetComponent<EnemyUnit>();

            enemyUnit.HP -= supportAttack;
            currentSupportHP -= enemyUnit.Attack;
            float hpRate = (float)currentSupportHP / fullSupportHP;
            hpBar.fillAmount = hpRate;

            //사망처리
            if (currentSupportHP <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
