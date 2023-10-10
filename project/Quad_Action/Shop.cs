using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    Player enterPlayer;

    //상점 입장
    public void Enter(Player player)//플레이어의 정보를 받아야함
    {
        enterPlayer = player;
        uiGroup.anchoredPosition = Vector3.zero;//상점 UI를 띄운다.(-1000의 위치에서 0의 위치로 감)
    }

    //상점 나가기
    public void Exit()
    {
        anim.SetTrigger("doHello");
        uiGroup.anchoredPosition = Vector3.down*1000;//상점 UI를 원래 위치로
    }
    //아이템 구매
    public void Buy(int index)
    {

    }
}
