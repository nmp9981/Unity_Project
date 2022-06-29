using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMgr : MonoBehaviour
{

    [SerializeField]
    private GameObject item;

    FireBall fireball;
    public float min_res = 0.4f;
    public float max_res = 0.55f;
    private float width = 2.8f;//끝 x좌표의 절댓값
    private float start_y = 6f;//시작 y좌표
    // Start is called before the first frame update
    void Start()
    {
        fireball = GameObject.FindWithTag("FireBall").GetComponent<FireBall>();//FireBall 스크립트에서 변수 가져오기
        StartCoroutine(CreatitemRoutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator CreatitemRoutine()
    {
        while (true)
        {
            Creatitem();
            yield return new WaitForSeconds(UnityEngine.Random.Range(min_res, max_res));//생성 시간 랜덤
        }
    }

    private void Creatitem()
    {
        //랜덤 위치 생성
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-width, width), start_y, 0f);
        Instantiate(item, pos, Quaternion.identity);//불 생성 위치
    }
}
