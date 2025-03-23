using System.Collections;
using UnityEngine;

public class Projective : MonoBehaviour
{
    public float arrowMoveSpeed= 3;
    public Vector3 arrowMoveDir { get; set; }
    public float arrowMoveDistance { get; set; }

    private float arrowMaxMoveDistance = 15;

    //현재 타수
    public float hitNumber = 0;
    //스킬데미지
    public long skillDamageRate;

    UIManager uiManager;
    ObjectFullingInTest objectFulling;

    void Awake()
    {
        arrowMoveDistance = 0;
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        objectFulling = GameObject.Find("ObjectFulling").GetComponent<ObjectFullingInTest>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveArrow();
        DestroyArrow();
    }
    /// <summary>
    /// 화살 정보 초기화
    /// </summary>
    public void InitArrowInfo(Vector3 targetPos, Vector3 moveDir, bool isRange, int hitNum, long skillDamage)
    {
        //시야 범위내에 들어가면 타겟을 향하게
        if (isRange)
        {
            transform.LookAt(targetPos);
            arrowMoveDir = targetPos - gameObject.transform.position;
            arrowMoveSpeed = 3;
        }
        else//그렇지 않으면 직선으로
        {
            transform.LookAt(moveDir);
            arrowMoveDir = moveDir;
            arrowMoveSpeed = 15;
        }
        
        arrowMoveDistance = 0;
        hitNumber = hitNum;
        skillDamageRate = skillDamage;
    }
    /// <summary>
    /// 화살 이동
    /// </summary>
    void MoveArrow()
    {
        gameObject.transform.position += arrowMoveSpeed * arrowMoveDir * Time.deltaTime;
        arrowMoveDistance += arrowMoveSpeed*Time.deltaTime;
    }
    /// <summary>
    /// 화살 파괴
    /// </summary>
    void DestroyArrow()
    {
        if(arrowMoveDistance >= arrowMaxMoveDistance)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Contains("Monster"))
        {
            long maxDamage = (PlayerInfo.maxAttackDamage* skillDamageRate)/100;
            long minDamage = (maxDamage * PlayerInfo.workmanship) / 100;
            long damage = (long)Random.Range(minDamage,maxDamage);

            int criticalValue = Random.Range(0, 100);
            if(criticalValue < PlayerInfo.criticalRate)//크리 터짐
            {
                damage *= PlayerInfo.criticalDamageRate;
            }

            uiManager.ShowDamage(damage);
            if(criticalValue < PlayerInfo.criticalRate)//크리 터짐
            {
                ShowCriticalDamageAsSkin(damage, other.gameObject);
            }
            else
            {
                ShowDamageAsSkin(damage, other.gameObject);
            }
           
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 일반 데미지 보이기
    /// </summary>
    /// <param name="Damage">데미지</param>
    /// <param name="monsterPos">몬스터 위치</param>
    void ShowDamageAsSkin(long Damage, GameObject monsterPos)
    {
        string damageString = Damage.ToString();
        float damageLength = uiManager.damageImage[0].bounds.size.x * damageString.Length;
        Bounds bounds = monsterPos.GetComponent<MeshRenderer>().bounds;
        Vector3 damageStartPos = bounds.center + Vector3.up * (bounds.size.y *0.5f + 1)+damageLength*Vector3.left*0.25f;
        damageStartPos += Vector3.up * hitNumber* uiManager.damageImage[0].bounds.size.y*0.55f;

        for (int i = 0; i < damageString.Length; i++)
        {
            GameObject damImg = objectFulling.MakeObj(damageString[i]-'0');
            damImg.transform.position = damageStartPos+Vector3.right * uiManager.damageImage[0].bounds.size.x*i*0.5f;
        }
        KeyInputSystem.orderSortNum += 1;
    }

    /// <summary>
    /// 크리 데미지 보이기
    /// </summary>
    /// <param name="Damage">데미지</param>
    /// <param name="monsterPos">몬스터 위치</param>
    void ShowCriticalDamageAsSkin(long Damage, GameObject monsterPos)
    {
        string damageString = Damage.ToString();
        float damageLength = uiManager.criticalDamageImage[0].bounds.size.x * damageString.Length;
        Bounds bounds = monsterPos.GetComponent<MeshRenderer>().bounds;
        Vector3 damageStartPos = bounds.center + Vector3.up * (bounds.size.y*0.5f+ 1) + damageLength * Vector3.left * 0.25f;
        damageStartPos += Vector3.up * hitNumber * uiManager.criticalDamageImage[0].bounds.size.y*0.5f;

        for (int i = 0; i < damageString.Length; i++)
        {
            GameObject damImg = objectFulling.MakeObj((damageString[i] - '0')+10);
            damImg.transform.position = damageStartPos + Vector3.right * uiManager.criticalDamageImage[0].bounds.size.x * i * 0.5f;
        }
        KeyInputSystem.orderSortNum += 1;
    }
}
