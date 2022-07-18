using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterImage : MonoBehaviour
{
    Manager manager;
    SpriteRenderer mobImage;
    public Sprite[] mobSprite;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        mobImage = gameObject.GetComponent<SpriteRenderer>();
        mobSprite = Resources.LoadAll<Sprite>("mob");//이미지 불러오기
        mobImage.sprite = mobSprite[0];//이미지 배열
    }

    // Update is called once per frame
    void Update()
    {
        mobImage.sprite = mobSprite[manager.lv-1];//레벨에 맞는 이미지
    }
}
