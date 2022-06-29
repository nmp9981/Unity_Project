using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMgr : MonoBehaviour
{

    [SerializeField]
    private GameObject item2;

    FireBall fireball;
   
    public float min_res = 1.2f;
    public float max_res = 2.0f;
    private float width = 2.8f;//끝 x좌표의 절댓값
    private float start_y = 6f;//시작 y좌표
    // Start is called before the first frame update
    void Start()
    {
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
        StartCoroutine(Creatitem2Routine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator Creatitem2Routine()
    {
        while (true)
        {
            Creatitem2();
            yield return new WaitForSeconds(UnityEngine.Random.Range(min_res, max_res));//생성 시간 랜덤
        }
    }

    private void Creatitem2()
    {
        //Vector3 pos = Camera.main.ViewportToWorldPoint(new Vector3(UnityEngine.Random.Range(0.0f, 1.0f), 1.1f, 0));//오브젝트가 화면 내에 있도록 제한
        //pos.z = 0.0f;
        //랜덤위치 생성
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-width, width), start_y, 0f);
        Instantiate(item2, pos, Quaternion.identity);//돌 생성 위치
    }
    
}
