using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSetting : MonoBehaviour
{
    public GameObject card;
    public GameObject[] cards;

    int cardMax = 27;
    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.SetActive(true);
        cards = new GameObject[cardMax];
        //DB에서 정보를 받음

        //오브젝트 생성
        MakeObject();
    }

   
    //오브젝트 배치
    void MakeObject()
    {
        for(int i = 0; i < cardMax; i++)
        {
            cards[i] = Instantiate(card, card.transform.position, card.transform.rotation);
            cards[i].GetComponentInChildren<Button>().transform.position = new Vector3(150+200*(i%9),300+ 300*(i/9), 0);
            cards[i].GetComponent<CardClick>().cardName = i.ToString();
            cards[i].SetActive(true);
        }
    }
}
