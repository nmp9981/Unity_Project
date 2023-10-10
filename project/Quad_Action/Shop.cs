using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public RectTransform uiGroup;
    public Animator anim;

    public GameObject[] itemObj;//아이템 오브젝트 배열
    public int[] itemPrice;//아이템 가격
    public Transform[] itemPos;//생성 위치
    public string[] talkData;//대사
    public Text talkText;//대사

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
        int price = itemPrice[index];
        if(price > enterPlayer.coin)//금액 부족
        {
            StopCoroutine(Talk());//1개만 띄워야하므로 이미 실행중인 코루틴은 중지
            StartCoroutine(Talk());//대사 띄우기
            return;
        }
        enterPlayer.coin -= price;//구매

        //구입 성공시 아이템 생성
        Vector3 ranVec = Vector3.right * Random.Range(-3, 3)+ Vector3.forward * Random.Range(-3, 3);//약간 랜덤한 위치
        Instantiate(itemObj[index],itemPos[index].position+ranVec,itemPos[index].rotation);
    }
    //2초 지난뒤 원래 데이터로
    IEnumerator Talk()
    {
        talkText.text = talkData[1].ToString();//변경된 대사
        yield return new WaitForSeconds(2f);
        talkText.text = talkData[0].ToString();//원래 대사
    }
}
