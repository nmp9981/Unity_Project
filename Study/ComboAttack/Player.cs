using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Animator anim;
    public Slider silder;

    public int speed;
    public float minPos;
    public float maxPos;
    public RectTransform pass;

    public int atkNum;
    bool isAtk = false;//공격 중인가?
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isAtk)
        {
            isAtk = true;
            SetAtk();
        }
    }
    public void PlayAnimation(int atkNum)
    {
        anim.SetFloat("Blend", atkNum);
        anim.SetTrigger("Atk");
    }
    public void SetAtk()
    {
        silder.value = 0f;
        //UI 위치 설정
        minPos = pass.anchoredPosition.x;
        maxPos = minPos + pass.sizeDelta.x;
        StartCoroutine(ComboAtk());
    }
    IEnumerator ComboAtk()
    {
        yield return null;
        while (!(Input.GetKeyDown(KeyCode.Space) || silder.value == silder.maxValue))
        {
            silder.value += Time.deltaTime * speed;
            yield return null;
        }
        if(silder.value >= minPos && silder.value <= maxPos)//타이밍이 맞음
        {
            PlayAnimation(atkNum++);
            if (atkNum < 4) SetAtk();//다시 콤보 공격
            else
            {
                atkNum = 0;//초기화
                isAtk = false;
            }
        }
        else//타이밍이 틀림
        {
            PlayAnimation(0);
            atkNum = 0;
            isAtk = false;
        }
        silder.value = 0;
    }
}
