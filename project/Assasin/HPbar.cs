using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    //public Monster monster;
    Image HealthBar;//체력바 이미지
    public float HPRate;//hp 비율
    Monster monster;

    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.FindWithTag("KingSlime").GetComponent<Monster>();//Monster스크립트에서 변수 가져오기
        HealthBar = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        HPRate = (float) monster.Current_HP / (float) monster.Max_HP;
        HealthBar.fillAmount = HPRate;
    }
}
