using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    public GameObject timerText;//시간 관리
    public GameObject pointText;//점수 관리
    float time = 30.0f;
    int point = 0;
    public GameObject generator;

    //득점
    public void GetApple()
    {
        this.point += 100;
    }
    //실점
    public void GetBomb()
    {
        this.point /= 2;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.generator = GameObject.Find("ItemGernerator");
        this.timerText = GameObject.Find("Time");
        this.pointText = GameObject.Find("Point");
    }

    // Update is called once per frame
    void Update()
    {
        this.time -= Time.deltaTime;
        
        //시간에 따른 난이도 조절
        if(this.time < 0)//종료
        {
            this.time = 0;
            this.generator.GetComponent<ItemGenerator>().SetParameter(10000.0f, 0, 0);
        }else if(this.time >=0 && this.time < 5)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.7f, -0.04f, 3);
        }
        else if (this.time >= 5 && this.time < 12)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.5f, -0.05f, 6);
        }
        else if (this.time >= 12 && this.time < 23)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(0.8f, -0.04f, 4);
        }
        else if (this.time >= 23 && this.time < 30)
        {
            this.generator.GetComponent<ItemGenerator>().SetParameter(1.0f, -0.03f, 2);
        }

        this.timerText.GetComponent<Text>().text = this.time.ToString("F1");//소수점 첫째자리까지 표시
        this.pointText.GetComponent<Text>().text = this.point.ToString()+" Point";//소수점 첫째자리까지 표시
    }
}
