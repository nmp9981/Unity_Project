using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitDamageText;
    [SerializeField] GameObject tombStone;
    [SerializeField] GameObject tombStoneMessage;
   
    // Start is called before the first frame update
    void Awake()
    {
        hitDamageText.text = "";
        tombStone.SetActive(false);
        tombStoneMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster")
        {
            int hit = collision.gameObject.GetComponent<MonsterFunction>().monsterHitDamage;
            int finalHit = Random.Range(hit * 90 / 100, hit * 110 / 100);
            GameManager.Instance.PlayerHP -= finalHit;
            StartCoroutine(ShowDamage(finalHit));

            if(GameManager.Instance.PlayerHP <= 0)//캐릭터 사망
            {
                GameManager.Instance.IsCharacterDie = true;
                tombStone.SetActive(true);
                tombStoneMessage.SetActive(true);
            }
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage(int finalHit)
    {
        hitDamageText.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 2f, 0));
        hitDamageText.text = finalHit.ToString();
        yield return new WaitForSeconds(0.3f);
        hitDamageText.text = "";
    }
    //부활
    public void Resurrection()
    {
        GameManager.Instance.IsCharacterDie = false;
        GameManager.Instance.PlayerHP = GameManager.Instance.PlayerMaxHP / 2;
        this.gameObject.transform.position = PortalManager.PortalInstance.portalist[0].transform.position;//마을로 이동
        tombStone.SetActive(false);
        tombStoneMessage.SetActive(false);
    }
}
