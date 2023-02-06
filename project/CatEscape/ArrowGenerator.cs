using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowGenerator : MonoBehaviour
{
    public GameObject arrowPrefab;
    float span = 0.4f;//쿨타임
    float delta = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;//경과시간
        if (this.delta > span)
        {
            this.delta = 0;
            GameObject go = Instantiate(arrowPrefab) as GameObject;//화살 생성(매개변수로 프리팹 전달),GameObject로 강제 형 변환
            int px = Random.Range(-6, 7);//위치 랜덤
            go.transform.position = new Vector3(px, 7, 0);
        }
    }
}
