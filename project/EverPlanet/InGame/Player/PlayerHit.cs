using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI hitDamageText;
    // Start is called before the first frame update
    void Awake()
    {
        hitDamageText.text = "";
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
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage(int finalHit)
    {
        hitDamageText.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 2f, 0));
        Debug.Log(finalHit);
        hitDamageText.text = finalHit.ToString();
        yield return new WaitForSeconds(0.3f);
        hitDamageText.text = "";
    }
}
