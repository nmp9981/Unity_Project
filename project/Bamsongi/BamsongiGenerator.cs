using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BamsongiGenerator : MonoBehaviour
{
    public GameObject bamsongiPrefab;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))//눌렀을때 한번만
        {
            GameObject bamsongi = Instantiate(bamsongiPrefab) as GameObject;//형변환, 밤송이 프리팹 오브젝트 생성

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//스크린 좌표를 월드 좌표 벡터로 변환
            Vector3 worldDir = ray.direction;//방향 벡터
            bamsongi.GetComponent<BamsongiController>().Shoot(worldDir.normalized*1800);//BamsongiController 스크립트의 Shoot()불러오기
        }
    }
}
