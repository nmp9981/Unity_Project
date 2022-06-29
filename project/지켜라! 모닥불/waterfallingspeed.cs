using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class waterfallingspeed : MonoBehaviour
{
    item water;
    FireMgr fireMgr;
    Strength strength;

    Image thisimg;
    public Sprite speed_decrease;
    public Sprite item_increase;

    public int lv = 1;//강화 레벨
    private int num;//랜덤 번호
    private float LimitMinSpeed = 1.5f;//최소 속도
    private float MinMaxiBetween = 0.5f;//최소 최대 간격
    private float MinMiniBetween = 0.05f;//최소 최소 간격

    void Start()
    {
        thisimg = GetComponent<Image>();

        water = GameObject.FindWithTag("water").GetComponent<item>(); 
        fireMgr = GameObject.FindWithTag("watermgr").GetComponent<FireMgr>();
        strength = GameObject.FindWithTag("enchant").GetComponent<Strength>();
        num = Random.Range(1, 3);

        InvokeRepeating("WaterCountUP", 7.0f, 5.0f); //시작 7초뒤 부터 5.0f 마다 "BalanceTime" 함수 반복
    }
    void Update()
    {
        if (num == 1)
        {
            thisimg.sprite = speed_decrease;
        }else if (num == 2)
        {
            thisimg.sprite = item_increase;
        }
    }
    public void onclick()
    {
        if (num == 1)
        {
            lv++;
            Slowerspeed();
            num = Random.Range(1, 3);
            strength.enchant.SetActive(false);
        }
        else if (num == 2)
        {
            lv++;
            DecreaseItem();
            num = Random.Range(1, 3);
            strength.enchant.SetActive(false);
        }
    }
    //물방울 속도 감소
    public void Slowerspeed()
    {
        water.itemspeed = Mathf.Max(water.itemspeed - 0.5f,LimitMinSpeed);
        water.fallingitem.velocity = Vector3.down * water.itemspeed;
    }
    //물방울 개수 감소
    public void DecreaseItem()
    {
        fireMgr.max_res = fireMgr.max_res + 0.1f;
    }
    //시간이 지날수록 물방울의 개수 증가
    void WaterCountUP()
    {
        fireMgr.min_res = Mathf.Max(fireMgr.min_res - 0.1f, MinMiniBetween);//0.1초 밑으로 떨어질 수 없음
    }
}
