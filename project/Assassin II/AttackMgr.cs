using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackMgr : MonoBehaviour
{
    Manager manager;
    CSVReader CSVReader;//csv파일 읽기
    List<Dictionary<string, object>> data;

    [SerializeField] public GameObject Dagger;//표창 객체
    public Transform draggerPos;//위치 지정

    public Text DamageText;//데미지 텍스트
    public int Clicks;//데미지

    // Start is called before the first frame update
    void Start()
    {
        data = CSVReader.Read("UserDataA"); //엑셀데이터 불러오기
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
    }

    void Update()
    {
        Clicks = (int)data[manager.lv - 1]["공격력"];
    }
    public void ProduceWeapon()
    {
        Instantiate(Dagger, draggerPos.position, draggerPos.rotation);//발사되는 위치 초기화
    }
}
