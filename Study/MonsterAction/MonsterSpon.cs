using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spon : MonoBehaviour
{
    [SerializeField] GameObject mobPrefab;
    public GameObject[] Wall;

    int mobCount;
    // Start is called before the first frame update
    void Start()
    {
        mobCount = 0;
    }

    void MonsterSpon()
    {
        GameObject mob = Instantiate(mobPrefab) as GameObject;//화살 생성(매개변수로 프리팹 전달),GameObject로 강제 형 변환
        int px = Random.Range(-3, 3);//위치 랜덤
        mob.transform.position = new Vector3(px, -4, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(mobCount < 4)
            {
                MonsterSpon();
                mobCount++;
            }
        }
    }
}
