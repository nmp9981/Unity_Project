using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    //이동 속도
    [Range(100, 800)]
    public float speed;
   
    Rigidbody2D rigid2D;

    //공의 속도
    [SerializeField]
    TextMeshProUGUI ballVelocityText;
    [SerializeField]
    TextMeshProUGUI ballDistanceText;

    //측정 지점
    GameObject flagObj;

    void Awake()
    {
        ComponentCall();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        BallVelocityText();
        BallDistText();
    }
    /// <summary>
    /// 기능 : 컴포넌트 불러오기
    /// </summary>
    void ComponentCall()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        ballVelocityText = GameObject.Find("Velocity").GetComponent<TextMeshProUGUI>();
        ballVelocityText.rectTransform.anchoredPosition = new Vector2(-800, -450);

        ballDistanceText = GameObject.Find("Dist").GetComponent<TextMeshProUGUI>();
        ballDistanceText.rectTransform.anchoredPosition = new Vector2(800, -450);

        flagObj = GameObject.Find("Flag");
    }

    /// <summary>
    /// 기능 : 공 이동
    /// 1) 좌우 방향으로만 이동
    /// </summary>
    private void Move()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Vector3 moveVec = new Vector3(1, 0, 0).normalized;//이동 방향, 정규화(대각선에서 더 빨라지는거 방지)
            rigid2D.AddForce(moveVec * speed);
        }
        //입력(-1~1 반환)
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

    }
    /// <summary>
    /// 기능 : 오브젝트 속도 표시
    /// 1) 자연수로 표시되게
    /// 2) 단위는 Km/h
    /// </summary>
    void BallVelocityText()
    {
        float velocity = rigid2D.velocity.magnitude*3.6f;
        velocity = Mathf.Round(velocity);//자연수로
        ballVelocityText.text = velocity.ToString()+" Km/h";
    }
    void BallDistText()
    {
        float dist = Mathf.Max(0,this.gameObject.transform.position.x - flagObj.transform.position.x);
        dist = Mathf.Round(dist);//자연수로
        ballDistanceText.text = dist.ToString() + " m";
    }
}
