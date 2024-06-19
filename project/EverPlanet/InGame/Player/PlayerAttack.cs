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
        if (Input.GetKeyDown(KeyCode.X) && curTime >= coolTime && GameManager.Instance.PlayerMP>=11)
        {
            StartCoroutine(ShotDrag());
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
    IEnumerator ShotDrag()
    {
        anim.SetBool("BasicAttack", true);
        GameManager.Instance.PlayerMP -= 11;
       
        for (int i = 0; i < 3; i++)
        {
            GameObject gm = objectfulling.MakeObj(2);
            gm.transform.position = startDragPosition.transform.position;//캐릭터 위치에서 날리기 시작

            if(gameObject.name == "Player") gm.GetComponent<DragFunction>().isShadow = false;//쉐파 여부에 따른 공격력
            else gm.GetComponent<DragFunction>().isShadow = true;//쉐파 여부에 따른 공격력

            yield return new WaitForSeconds(0.1f);
        }
        anim.SetBool("BasicAttack", false);
    }
    IEnumerator ShotAvenger()
    {
        anim.SetBool("BasicAttack", true);
        GameManager.Instance.PlayerMP -= 30;

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
}
