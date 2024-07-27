using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitDamageText;
    [SerializeField] GameObject tombStone;
    [SerializeField] GameObject tombStoneMessage;

    void Awake()
    {
        hitDamageText.text = "";
        tombStone.SetActive(false);
        tombStoneMessage.SetActive(false);
    }
    private void Update()
    {
        PlayerDie();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster" || collision.gameObject.tag == "Bear")
        {
            int avoidRandom = Random.Range(0, 100);
            int hit = 0;
            
            if(collision.gameObject.tag == "Bear")
            {
                if (collision.gameObject.GetComponent<BearBossFunction>()) hit = collision.gameObject.GetComponent<BearBossFunction>().monsterHitDamage;
                else hit = 2500;
            }
            else hit = collision.gameObject.GetComponent<MonsterFunction>().monsterHitDamage;
            int finalHit = (avoidRandom<GameManager.Instance.PlayerAvoid)?0:Random.Range(hit * 90 / 100, hit * 110 / 100);
            GameManager.Instance.PlayerHP -= finalHit;
            StartCoroutine(ShowDamage(finalHit));
        }
    }
    //사망
    void PlayerDie()
    {
        if (GameManager.Instance.PlayerHP <= 0)//캐릭터 사망
        {
            GameManager.Instance.IsCharacterDie = true;
            tombStone.SetActive(true);
            tombStoneMessage.SetActive(true);
        }
    }
    //데미지 보여주기
    public IEnumerator ShowDamage(int finalHit)
    {
        hitDamageText.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 2f, 0));
        hitDamageText.text = (finalHit==0)?"MISS": finalHit.ToString();
        yield return new WaitForSeconds(0.3f);
        hitDamageText.text = "";
    }
    //부활
    public void Resurrection()
    {
        GameManager.Instance.IsCharacterDie = false;
        GameManager.Instance.PlayerHP = GameManager.Instance.PlayerMaxHP / 2;
        GameManager.Instance.PlayerEXP = Mathf.Max(0, (int)GameManager.Instance.PlayerEXP-(int)GameManager.Instance.PlayerReqExp /20);
        this.gameObject.transform.position = PortalManager.PortalInstance.portalist[0].transform.position;//마을로 이동
        tombStone.SetActive(false);
        tombStoneMessage.SetActive(false);
    }
}
