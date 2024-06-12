using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    GameObject player;
    GameObject target;
    Transform monsterTarget;//가장 가까운 몬스터의 위치
    float moveDist;//표창 이동거리
    Vector3 moveVec;

    const float distMax = 900;

    [SerializeField] TextMeshProUGUI DamegeText;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        player = GameObject.Find("Body05");
        target = GameObject.Find("DragTarget");
    }
    private void OnEnable()
    {
        NearMonster();
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
    }
    void Update()
    {
        DragMove();
    }
    //가장 가까운 몹 찾기
    void NearMonster()
    {
        monsterTarget = null;
        float betweenDist = distMax;//캐릭터와 몬스터 간 거리
        Vector3 seeVector = (target.transform.position - player.transform.position).normalized;//시야 벡터

        foreach (var gm in MonsterSpawner.spawnMonster)
        {
            float newDist = (gm.transform.position - this.gameObject.transform.position).sqrMagnitude;
            Vector3 monsterVector = (gm.transform.position - player.transform.position).normalized;//캐릭터와 몬스터간 방향

            if (!MonsterInPlayerSee(seeVector,monsterVector)) continue;//캐릭터 시야내에 없음
            if (betweenDist > newDist)
            {
                betweenDist = newDist;
                monsterTarget = gm.transform;
            }
        }
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
        if (moveDist > distMax)
        {
            gameObject.SetActive(false);
        }
    }
    //캐릭터 시야와 몬스터 간 각도
    bool MonsterInPlayerSee(Vector3 seeVector, Vector3 monsterVector)
    {
        seeVector.y = 0;
        monsterVector.y = 0;

        float dot = seeVector.x * monsterVector.x + seeVector.z * monsterVector.z;
        float cosTheta = dot / (seeVector.magnitude * monsterVector.magnitude);
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;

        if (Mathf.Abs(theta) <= 60f) return true;
        return false;
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
            DamegeText.transform.position = Camera.main.WorldToScreenPoint(collision.gameObject.transform.position + new Vector3(0, 1f, 0));
            DamegeText.text = GameManager.Instance.PlayerAttack.ToString();
            collision.gameObject.GetComponent<MonsterFunction>().monsterHP -= GameManager.Instance.PlayerAttack;
            StartCoroutine(ShowDamage());
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage()
    {
        yield return new WaitForSeconds(0.3f);
        DamegeText.text = "";
        gameObject.SetActive(false);
    }
}
