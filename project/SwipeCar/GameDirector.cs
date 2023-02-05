using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    GameObject car;
    GameObject flag;
    GameObject distance;
    // Start is called before the first frame update
    void Start()
    {
        this.car = GameObject.Find("car");
        this.flag = GameObject.Find("flag");
        this.distance = GameObject.Find("Distance");
    }

    // Update is called once per frame
    void Update()
    {
        float length = (this.flag.transform.position.x - this.car.transform.position.x)*200;
        if (length >= 0)//성공
        {
            this.distance.GetComponent<Text>().text = "목표 지점까지 " + length.ToString("F0") + "M";//소수점 2자리까지
        }
        else//실패
        {
            this.distance.GetComponent<Text>().text = "Game Over!";
        }
    }
}
