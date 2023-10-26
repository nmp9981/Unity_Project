using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardClick : MonoBehaviour
{
    public enum Type {Who,What,Why};//카드 종류

    public Type cardType;//카드 타입
    public GameObject outLine;//바깥 선
    public Text cardNameText;//카드 명
    public string cardName;//카드 명
    // Start is called before the first frame update
    void Start()
    {
        outLine.SetActive(false);
        cardNameText.text = cardName;
    }

    //마우스 클릭
    public void Click()
    {
        if (outLine.activeSelf) outLine.SetActive(false);
        else
        {
            outLine.transform.position = this.gameObject.GetComponentInChildren<Button>().transform.position;
            outLine.SetActive(true);
            //클릭한 정보를 저장
        }
    }
}
