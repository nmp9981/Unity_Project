using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHit : MonoBehaviour
{
    CSVReader CSVReader;//csv파일 읽기

    public int userAttack;//공격력
    public int lv;//레벨

    // Start is called before the first frame update
    void Start()
    {
        List<Dictionary<string, object>> data = CSVReader.Read("UserDataA"); //엑셀데이터 불러오기
        userAttack = (int)data[0]["공격력"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
