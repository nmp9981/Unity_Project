using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerAttack : MonoBehaviour
{
    ObjectFulling objectfulling;
    GameObject target;
    [SerializeField] Transform startDragPosition;
    [SerializeField] Animator anim;

    float curTime = 0;
    float coolTime;
    int[] mpCost = new int[4]{0,0,16,22};
    int attackCount = 0;
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        target = GameObject.Find("DragTarget");
        coolTime = GameManager.Instance.PlayerAttackSpeed;
    }

    void Update()
    {
        Attack();
        TimeFlow();
    }
    void Attack()
    {
        if (GameManager.Instance.IsCharacterDie) return;
        if (Input.GetKeyDown(KeyCode.LeftControl) && curTime >= coolTime)//일반 공격
        {
            StartCoroutine(ShotDrag(1));
            curTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.Z) && curTime >= coolTime && GameManager.Instance.PlayerMP >= mpCost[2])
        {
            StartCoroutine(ShotDrag(2));
            curTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.X) && curTime >= coolTime && GameManager.Instance.PlayerMP >= mpCost[3])
        {
            StartCoroutine(ShotDrag(3));
            curTime = 0;
        }
        if (Input.GetKeyDown(KeyCode.C) && curTime >= coolTime && GameManager.Instance.PlayerMP >= 30)
        {
            StartCoroutine(ShotAvenger());
            curTime = 0;
        }
    }
    void TimeFlow()
    {
        curTime += Time.deltaTime;
    }
    IEnumerator ShotDrag(int throwCount)
    {
        anim.SetBool("BasicAttack", true);
        GameManager.Instance.PlayerMP -= mpCost[throwCount];
        SoundManager._sound.PlaySfx(4);
        attackCount = (attackCount+1)%10;
        for (int i = 0; i < throwCount; i++)
        {
            GameObject gm = objectfulling.MakeDragObj(2, 7*attackCount);//여기가 문제
            gm.transform.position = startDragPosition.transform.position;//캐릭터 위치에서 날리기 시작
            DragFunction dragFunction = gm.GetComponent<DragFunction>();
            dragFunction.skillDigit = throwCount;//럭키세븐인지 트리플스로우인지 일반 공격인지 구분
            dragFunction.monsterTarget = NearMonster();//가장 가까운 몬스터
            dragFunction.OnEnableDrag();

            if(gameObject.name == "Player") gm.GetComponent<DragFunction>().isShadow = false;//쉐파 여부에 따른 공격력
            else gm.GetComponent<DragFunction>().isShadow = true;//쉐파 여부에 따른 공격력
            yield return new WaitForSeconds(0.05f);
        }
        anim.SetBool("BasicAttack", false);
    }
    IEnumerator ShotAvenger()
    {
        anim.SetBool("BasicAttack", true);
        GameManager.Instance.PlayerMP -= 30;
        SoundManager._sound.PlaySfx(4);
        for (int i = 0; i < 1; i++)
        {
            GameObject gm = objectfulling.MakeObj(1);
            gm.transform.position = startDragPosition.transform.position;//캐릭터 위치에서 날리기 시작
            gm.transform.rotation = Quaternion.Euler(90, 0, 0);

            if (gameObject.name == "Player") gm.GetComponent<AvengerSkill>().isShadow = false;//쉐파 여부에 따른 공격력
            else gm.GetComponent<AvengerSkill>().isShadow = true;//쉐파 여부에 따른 공격력

            yield return new WaitForSeconds(0.05f);
        }
        anim.SetBool("BasicAttack", false);
    }
    //가장 가까운 몹 찾기
    public Transform NearMonster()
    {
        Transform monsterTarget = null;
        float betweenDist = GameManager.Instance.ThrowDist;//캐릭터와 몬스터 간 거리
        Vector3 seeVector = (target.transform.position - gameObject.transform.position).normalized;//시야 벡터

        foreach (var gm in MonsterSpawner.spawnMonster)
        {
            float newDist = (gm.transform.position - this.gameObject.transform.position).magnitude;
            Vector3 monsterVector = (gm.transform.position - gameObject.transform.position).normalized;//캐릭터와 몬스터간 방향

            if (!MonsterInPlayerSee(seeVector, monsterVector)) continue;//캐릭터 시야내에 없음
            if (betweenDist > newDist)
            {
                betweenDist = newDist;
                monsterTarget = gm.transform;
            }
        }
        return monsterTarget;
    }
    //캐릭터 시야와 몬스터 간 각도
    public bool MonsterInPlayerSee(Vector3 seeVector, Vector3 monsterVector)
    {
        seeVector.y = 0;
        monsterVector.y = 0;

        float dot = seeVector.x * monsterVector.x + seeVector.z * monsterVector.z;
        float cosTheta = dot / (seeVector.magnitude * monsterVector.magnitude);
        float theta = Mathf.Acos(cosTheta) * 180 / Mathf.PI;

        if (Mathf.Abs(theta) <= 60f) return true;
        return false;
    }
}
