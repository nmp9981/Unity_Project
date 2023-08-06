using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Upgrage : MonoBehaviour
{
    Manager manager;
    SkillManager skillManager;
    [SerializeField]GameObject messageBox;
    [SerializeField] GameObject skillIndex;
    Color tripleColor;
    Color doubleColor;
    Color avengerColor;

    // Start is called before the first frame update
    void Awake()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        skillManager = GameObject.Find("SkillManager").GetComponent<SkillManager>();
        tripleColor = skillManager.tripleSkill.color;
        tripleColor.a = 0.3f;
        doubleColor = skillManager.tripleSkill.color;
        doubleColor.a = 0.3f;
        avengerColor = skillManager.avengerSkill.color;
        avengerColor.a = 0.3f;
        messageBox.SetActive(false);//처음엔 메세지창 끄기
        skillIndex.SetActive(false);
    }

    public void Upgrade()
    {
        if(manager.lv>=3 && manager.meso >= 10 && doubleColor.a<0.8f)
        {
            manager.meso -= 10;
            doubleColor.a = 1;
            skillManager.doubleSkill.color = doubleColor;
        }
        else
        {
            OnMessage();
        }

        if (manager.lv >= 5 && manager.meso>=50 && doubleColor.a==1 && tripleColor.a<0.8f)
        {
            manager.meso -= 50;
            tripleColor.a = 1.0f;
            skillManager.tripleSkill.color = tripleColor;//색의 구조체 자체를 변경해야함(수정 내용 반영)
        }
        else
        {
            OnMessage();
        }
        if (manager.lv >= 10 && manager.meso >= 300 && doubleColor.a == 1 && avengerColor.a < 0.8f) 
        {
            manager.meso -= 0;
            avengerColor.a = 1.0f;
            skillManager.avengerSkill.color = avengerColor;//색의 구조체 자체를 변경해야함(수정 내용 반영)
        }
        else
        {
            OnMessage();
        }
    }
    //메세지 켜기
    void OnMessage()
    {
        messageBox.SetActive(true);
        Time.timeScale = 0.0f;//일시 정지
    }
    //메세지 끄기
    public void OffMessageBox()
    {
        messageBox.SetActive(false);
        Time.timeScale = 1.0f;//다시 정상 시간으로
    }
    //메세지 켜기(스킬창)
    public void OnSkillIndex()
    {
        skillIndex.SetActive(true);
        Time.timeScale = 0.0f;//일시 정지
    }
    //메세지 끄기(스킬창)
    public void OffSkillIndex()
    {
        skillIndex.SetActive(false);
        Time.timeScale = 1.0f;//다시 정상 시간으로
    }
}
