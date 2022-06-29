using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shadow : MonoBehaviour
{
    FireBall fireball;
    public GameObject S1;
    public GameObject S2;
    public GameObject S3;
    public GameObject S4;
    public GameObject S5;
    public GameObject S6;
    public GameObject S7;
    public GameObject S8;
    public GameObject S9;
    public GameObject S10;

    void Start()
    {
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
    }

    // Update is called once per frame
    void Update()
    {
        Shadow_set();
    }

    public void Shadow_set()
    {
        if (fireball.Rate_HP >= 0.1f) S10.SetActive(false);
        else S10.SetActive(true);
        if (fireball.Rate_HP >= 0.2f) S9.SetActive(false);
        else S9.SetActive(true);
        if (fireball.Rate_HP >= 0.3f) S8.SetActive(false);
        else S8.SetActive(true);
        if (fireball.Rate_HP >= 0.4f) S7.SetActive(false);
        else S7.SetActive(true);
        if (fireball.Rate_HP >= 0.5f) S6.SetActive(false);
        else S6.SetActive(true);
        if (fireball.Rate_HP >= 0.6f) S5.SetActive(false);
        else S5.SetActive(true);
        if (fireball.Rate_HP >= 0.7f) S4.SetActive(false);
        else S4.SetActive(true);
        if (fireball.Rate_HP >= 0.8f) S3.SetActive(false);
        else S3.SetActive(true);
        if (fireball.Rate_HP >= 0.9f) S2.SetActive(false);
        else S2.SetActive(true);
    } 
}
