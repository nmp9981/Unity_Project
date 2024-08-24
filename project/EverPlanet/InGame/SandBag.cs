using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SandBag : MonoBehaviour
{
    InGameUI inGameUI;
    GameObject player;

    public long csumDamage;//누적 데미지

    [SerializeField] TextMeshProUGUI monsterInfo;
    [SerializeField] TextMeshProUGUI[] hitDamage;

    void Awake()
    {
        inGameUI = GameObject.Find("UIManager").GetComponent<InGameUI>();
        player = GameObject.Find("Player");
    }
    private void OnEnable()
    {
        csumDamage = 0;
        foreach (var damage in hitDamage) damage.text = "";
    }
    void Update()
    {
        MonsterUISetting();
    }
    void MonsterUISetting()
    {
        //플레이어와 일정 거리 이상이면 UI가 안보이게
        float dist = (player.transform.position - this.gameObject.transform.position).sqrMagnitude;
        if (dist > 1800)
        {
            csumDamage = 0;
            monsterInfo.gameObject.SetActive(false);
        }
        else monsterInfo.gameObject.SetActive(true);

        for (int idx = 0; idx < hitDamage.Length; idx++) hitDamage[idx].transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, idx + 2f, 0));
        monsterInfo.text = $"Total : {csumDamage}";
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            for (int idx = 0; idx < hitDamage.Length; idx++)
            {
                if (hitDamage[idx].text == "")
                {
                    bool isShadow = collision.gameObject.GetComponent<DragFunction>().isShadow;
                    csumDamage += collision.gameObject.GetComponent<DragFunction>().attackDamage;
                    StartCoroutine(ShowDamage(hitDamage[idx], idx, isShadow, collision.gameObject));
                    return;
                }
            }
        }
        if (collision.gameObject.tag == "Avenger")
        {
            for (int idx = 0; idx < hitDamage.Length; idx++)
            {
                if (hitDamage[idx].text == "")
                {
                    bool isShadow = collision.gameObject.GetComponent<AvengerSkill>().isShadow;
                    csumDamage += collision.gameObject.GetComponent<DragFunction>().attackDamage;
                    StartCoroutine(ShowDamage(hitDamage[idx], idx, isShadow, collision.gameObject));
                    return;
                }
            }
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage(TextMeshProUGUI damage, int idx, bool isShadow, GameObject gm)
    {
        long finalDamage = 0;
        yield return new WaitForSeconds(0.03f);
        if (gm.tag == "Weapon")
        {
            finalDamage = gm.GetComponent<DragFunction>().attackDamage;
            if (gm.GetComponent<DragFunction>().isCritical) damage.color = Color.red;
            else damage.color = new Color(219f / 255f, 132f / 255f, 0);
        }
        else if (gm.tag == "Avenger")
        {
            finalDamage = gm.GetComponent<AvengerSkill>().attackDamage;
            if (gm.GetComponent<AvengerSkill>().isCritical) damage.color = Color.red;
            else damage.color = new Color(219f / 255f, 132f / 255f, 0);
        }

        damage.text = finalDamage.ToString();
        yield return new WaitForSeconds(0.4f);
        damage.text = "";
    }
}
