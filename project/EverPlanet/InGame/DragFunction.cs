using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    PlayerAttack playerAttack;
    GameObject player;
    GameObject target;
    Transform monsterTarget;//가장 가까운 몬스터의 위치
    float moveDist;//표창 이동거리
    Vector3 moveVec;

    [SerializeField] TextMeshProUGUI DamegeText;
    public bool isShadow;
    public long attackDamage;
    public float criticalNum;
    public bool isCritical;
    public int skillDigit;

    void Awake()
    {
        playerAttack = GameObject.Find("Player").GetComponent<PlayerAttack>();
        player = GameObject.Find("Body05");
        target = GameObject.Find("DragTarget");
    }
    private void OnEnable()
    {
        monsterTarget = playerAttack.NearMonster();

        moveDist = 0f;

        if (monsterTarget == null)
        {
            moveVec = (target.transform.position - player.transform.position).normalized;
            moveVec.y = 0f;
        }
        else
        {
            moveVec = (monsterTarget.transform.position - player.transform.position).normalized;
        }
        
        gameObject.transform.rotation = Quaternion.Euler(0, DotAngle(), DotZAngle());
        criticalNum = Random.Range(0, 100);
        if (criticalNum < GameManager.Instance.CriticalRate) isCritical = true;
        else isCritical = false;
    }
    void Update()
    {
        DragMove();
    }
    //표창 이동
    void DragMove()
    {
        if (monsterTarget == null)
        {
            gameObject.transform.position += moveVec * GameManager.Instance.PlayerDragSpeed * Time.deltaTime;
            moveDist += moveVec.sqrMagnitude;
        }
        else
        { 
            gameObject.transform.position = Vector3.MoveTowards(this.transform.position, monsterTarget.position, GameManager.Instance.PlayerDragSpeed * Time.deltaTime);
            moveDist += moveVec.sqrMagnitude;
        }
        
        //사정 거리 초과
        if (moveDist > GameManager.Instance.ThrowDist)
        {
            gameObject.SetActive(false);
        }
    }
    //표창 y축 회전 정도
    float DotAngle()
    {
        float dot = -moveVec.x;
        float cosTheta = dot / moveVec.magnitude;
        float theta = -Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
    //표창 z축 회전 각도
    float DotZAngle()
    {
        if (monsterTarget == null) return 90;
        float dot = moveVec.y;
        float cosTheta = dot / moveVec.magnitude;
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
    //피격
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster")//몬스터 공격
        {
            SoundManager._sound.PlaySfx(5);
            attackDamage = AttackDamage();
            if (isShadow) attackDamage = (long)((float)attackDamage*GameManager.Instance.ShadowAttack/100f);
            collision.gameObject.GetComponent<MonsterFunction>().monsterHP -= attackDamage;
        }
    }
    //공격 데미지
    public long AttackDamage()
    {
        long attackMaxDamage = skillDigit==2?((long)(GameManager.Instance.PlayerAttack * GameManager.Instance.LuckySevenCoefficient/100f))
            : (long)((float)(GameManager.Instance.PlayerAttack * GameManager.Instance.TripleThrowCoefficient/100f));
        int attackRate = Random.Range(GameManager.Instance.Proficiency, 100);
        if (isCritical) attackMaxDamage = (long)((float)attackMaxDamage*GameManager.Instance.CriticalDamage/100f);//크리 데미지
        return attackMaxDamage * (long)attackRate / 100;
    }
}
