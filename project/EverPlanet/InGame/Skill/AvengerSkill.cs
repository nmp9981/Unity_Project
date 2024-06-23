using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AvengerSkill : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    GameObject player;
    GameObject target;
   
    float moveDist;//표창 이동거리
    Vector3 moveVec;
    const float distMax = 1000;

    public bool isShadow;
    public int hitCount;
    public long attackDamage;
    public float criticalNum;
    public bool isCritical;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        player = GameObject.Find("Body05");
        target = GameObject.Find("DragTarget");
    }
    private void OnEnable()
    {
        moveDist = 0f;
        hitCount = 0;

        moveVec = (target.transform.position - player.transform.position).normalized;
        moveVec.y = 0f;

        criticalNum = Random.Range(0, 100);
        if (criticalNum >= GameManager.Instance.CriticalRate) isCritical = true;
        else isCritical = false;
    }
    void Update()
    {
        SizeUp();
        DragMove();
    }
    void SizeUp()
    {
        gameObject.transform.localScale = Vector3.Lerp(gameObject.transform.localScale, new Vector3(18,8,18), 3*Time.deltaTime);
    }
   
    //표창 이동
    void DragMove()
    {
        gameObject.transform.position += moveVec * GameManager.Instance.PlayerDragSpeed * Time.deltaTime;
        moveDist += moveVec.sqrMagnitude;

        //사정 거리 초과
        if (moveDist > distMax)
        {
            gameObject.SetActive(false);
        }
    }
    //피격
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Monster")//몬스터 공격
        {
            hitCount++;
            attackDamage = AttackDamage();
            if (isShadow) attackDamage = attackDamage / 2;
            collision.gameObject.GetComponent<MonsterFunction>().monsterHP -= attackDamage;

            if (hitCount >= 6) gameObject.SetActive(false);
        }
    }
    public long AttackDamage()
    {
        long attackMaxDamage = GameManager.Instance.PlayerAttack * 180 / 100;
        int attackRate = Random.Range(GameManager.Instance.Workmanship,100);
        if (isCritical) attackMaxDamage = attackMaxDamage * GameManager.Instance.CriticalDamage / 100;//크리 데미지
        return attackMaxDamage * (long)attackRate / 100;
    }
}
