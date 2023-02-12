using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    //음원 2개를 받아야한다.
    public AudioClip appleSE;
    public AudioClip bombSE;
    AudioSource aud;
    GameObject director;//director 스크립트

    // Start is called before the first frame update
    void Start()
    {
        this.director = GameObject.Find("GameDirector");//GameDirector 스크립트 가져오기
        this.aud = GetComponent<AudioSource>();//오디오 소스 컴포넌트 받기
    }

    void OnTriggerEnter(Collider other)//충돌 시
    {
        if (other.gameObject.tag == "Apple")
        {
            this.director.GetComponent<GameDirector>().GetApple();//GameDirector스크립트의 GetApple()호출
            this.aud.PlayOneShot(appleSE);
        }
        else
        {
            this.director.GetComponent<GameDirector>().GetBomb();
            this.aud.PlayOneShot(bombSE);
        }
        Destroy(other.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//마우스가 클릭된 좌표(스크린 좌표를 월드 좌표계로)
            RaycastHit hit;//out에 계속 메서드 내 값을 채워 변수로 반환해라
            if(Physics.Raycast(ray,out hit, Mathf.Infinity))//카메라 광선과 충돌 판정
            {
                float x = Mathf.RoundToInt(hit.point.x);//반올림
                float z = Mathf.RoundToInt(hit.point.z);
                transform.position = new Vector3(x, 0, z);//해당 좌표로 이동
            }
        }
    }
}
