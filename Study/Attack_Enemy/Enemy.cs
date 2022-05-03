using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int HP = 50;
    public Image HP_Bar;//이미지 지정

    private bool isOperationProcessing;
    private List<int> healthQueue;//리스트 저장

    void Awake()//초기화
    {
        healthQueue = new List<int>();
        isOperationProcessing = false;
    }
    void Update()
    {
        if(!isOperationProcessing && healthQueue.Count > 0)
        {
            isOperationProcessing = true;
            StartCoroutine(healthLerp(healthQueue[0]));//코루틴 문 추가, 실행하는 동안에 다른것도 실행가능
        }
    }
    public void get_damage(int amount)
    {
        if (isOperationProcessing)//동작중이면 추가하지 않고 반환
        {
            return;
        }
        healthQueue.Add(amount);//주고 싶은 데미지만큼 추가
        
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }
    public IEnumerator healthLerp(int amount)
    {
        float currentHP = HP;//현재 HP
        float goalHP = HP - amount;//목표 HP

        float t = 0f;

        while (Mathf.Abs(currentHP - goalHP) > Mathf.Epsilon)//HP가 0.000001 이상
        {
            yield return new WaitForEndOfFrame();

            t += Time.deltaTime;//프레임 도는 시간 누적
            currentHP = Mathf.Lerp(currentHP, goalHP, t);//선형보간 함수로 현재 HP구함
            HP_Bar.fillAmount = currentHP / 100.0f;//비율로
        }
        HP -= amount;
        healthQueue.RemoveAt(0);//큐의 맨 앞 제거

        isOperationProcessing = false;//동작 마무리
    }
}
