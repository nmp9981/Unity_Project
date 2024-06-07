using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    ObjectFulling objectfulling;
    [SerializeField] Transform startDragPosition;
    [SerializeField] Animator anim;

    float curTime = 0;
    float coolTime;
    void Awake()
    {
        objectfulling = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        coolTime = GameManager.Instance.PlayerAttackSpeed;
    }

    void Update()
    {
        Attack();
        TimeFlow();
    }
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.X) && curTime >= coolTime)
        {
            StartCoroutine(ShotDrag());
            curTime = 0;
        }
    }
    void TimeFlow()
    {
        curTime += Time.deltaTime;
    }
    IEnumerator ShotDrag()
    {
        anim.SetBool("BasicAttack", true);
        for (int i = 0; i < 3; i++)
        {
            GameObject gm = objectfulling.MakeObj(2);
            gm.transform.position = startDragPosition.transform.position;//캐릭터 위치에서 날리기 시작
            yield return new WaitForSeconds(0.2f);
        }
        anim.SetBool("BasicAttack", false);
    }
}
