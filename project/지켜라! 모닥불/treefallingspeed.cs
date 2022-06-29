using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class treefallingspeed : MonoBehaviour
{
    item2 tree;
    StoneMgr stoneMgr;
    Strength strength;
    
    Image thisimg;
    public Sprite speed_decrease;
    public Sprite item_increase;

    public int lv = 1;//강화 레벨
    private int num;//랜덤 번호
    private float LimitMinSpeed = 1.5f;//최소 속도
    private float MinMaxiBetween = 1.1f;//최소 최대 간격
    private float MinMiniBetween = 0.5f;//최소 최대 간격

    void Start()
    {
        thisimg = GetComponent<Image>();//이미지 받기

        tree = GameObject.FindWithTag("tree").GetComponent<item2>();
        stoneMgr = GameObject.FindWithTag("treemgr").GetComponent<StoneMgr>();
        strength = GameObject.FindWithTag("enchant").GetComponent<Strength>();
        num = Random.Range(1, 3);
        LimitMinSpeed = 1.5f;

        InvokeRepeating("ItemCountUP", 10.0f, 8.0f); //10초후 8.0f 마다 "BalanceTime" 함수 반복
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
    //마우스 클릭시
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
            IncreaseItem();
            num = Random.Range(1, 3);
            strength.enchant.SetActive(false);
        }
    }
    //아이템 속도 느려지게
    public void Slowerspeed()
    {
        tree.itemspeed = Mathf.Max(tree.itemspeed - 0.5f,LimitMinSpeed);
        tree.fallingitem.velocity = Vector3.down * tree.itemspeed;
    }
    //아이템 개수 증가
    public void IncreaseItem()
    {
        stoneMgr.max_res = Mathf.Max(stoneMgr.max_res-0.3f,MinMaxiBetween);//0.8초 밑으로 떨어질 수 없음
    }
    //시간이 지나면 아이템 개수 증가
    void ItemCountUP()
    {
        stoneMgr.min_res = Mathf.Max(stoneMgr.min_res - 0.2f, MinMiniBetween);//0.4초 밑으로 떨어질 수 없음
    }
}
