using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Store : MonoBehaviour
{
    Manager manager;
    [SerializeField] GameObject store;

    private int redPossionValue = 3000;
    private int bluePossionValue = 45000;
    // Start is called before the first frame update
    void Awake()
    {
        store.SetActive(false);
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
    }

    public void OpenStore()
    {
        if (store.activeSelf == false)  store.SetActive(true);//상점 개시
        else store.SetActive(false);//상점 끄기
    }
    //빨간 포션 구매
    public void BuyRedPosion()
    {
        if (manager.meso < redPossionValue) return;//돈 부족
        manager.meso -= redPossionValue;
        manager.curExp += 50000;
    }
    //파란 포션 구매
    public void BuyBluePosion()
    {
        if (manager.meso < bluePossionValue) return;//돈 부족
        manager.meso -= bluePossionValue;
        manager.curExp += manager.requestEXP*15 / 100;//15%증가
    }
}
