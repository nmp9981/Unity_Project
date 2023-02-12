using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public GameObject applePrefab;
    public GameObject bombPrefab;

    float span = 1.0f;//생성 시간
    float delta = 0;//쿨타임
    int ratio = 2;//폭탄 생성 기준
    float speed = -0.03f;//speed

    //매개변수 일괄설정
    public void SetParameter(float span, float speed,int ratio)
    {
        this.span = span;
        this.speed = speed;
        this.ratio = ratio;
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > span)//span초마다 객체 생성
        {
            this.delta = 0;
            GameObject item;
            int dice = Random.Range(1, 11);
            if(dice <= this.ratio)//사과 생성
            {
                item = Instantiate(bombPrefab) as GameObject;//형변환
            }
            else///폭탄 생성
            {
                item = Instantiate(applePrefab) as GameObject;//형변환
            }
            
            //랜덤 위치
            float x = Random.Range(-1, 2);
            float z = Random.Range(-1, 2);
            item.transform.position = new Vector3(x, 4, z);//생성 위치정하기
            item.GetComponent<ItemController>().dropSpeed = this.speed;//ItemController스크립트의 dropSpeed변수 가져오기
        }
        
    }
}
