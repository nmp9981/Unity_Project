using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireBall : MonoBehaviour
{
    Rigidbody2D fireballRigid;

    [Header("FireBall")]
    [SerializeField] public float FireBall_HP = 10;
    [SerializeField] public int FireBall_Lv = 1;//레벨
    [SerializeField] float Player_Exp = 5;//회득 경험치

    private float FireBall_Y;//불의 y위치
    public bool LvUP = false;//레벨업 여부
    public float Current_HP;//현재 HP
    public float Current_EXP;//현재 EXP
    public float Rate_HP;//HP비율
    public float[] Require_EXP = new float[] { 0, 30, 50, 80, 140, 210000 };//요구 exp배열
    Color[] FireballColor = new Color[] { Color.red, new Color(1.0f,0.5f,0f),Color.yellow,Color.white,Color.blue };//색 배열

    [Header("HPManage")]
    [SerializeField] public float water_damage;//피격데미지
    [SerializeField] public float item_heal;//회복량
    [SerializeField] public float exp_boundary = 0.3f;//경험치 상승 기준점

    SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        FireBall_Lv = 1;//시작 레벨
        Current_HP = FireBall_HP;//처음엔 풀피
        fireballRigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        LevelUP();
        SizeHP();
        if (Current_HP <= 0f)
        {
            Time.timeScale = 0;
            Invoke("Game_Quit" ,2f);//죽었을때 2초뒤에 종료
        }

    }
    
    //레벨업
    void LevelUP()
    {
        LvUP = false;
        if (Rate_HP >= exp_boundary)
        {
            Current_EXP += Time.deltaTime * Player_Exp;//초당 5획득
        }
        
        if (Current_EXP >= Require_EXP[FireBall_Lv])//요구 경험치를 넘으면
        {
            LvUP = true;//레벨업 표시
            FireBall_Lv += 1;//레벨업
            Current_EXP = 0;//경험치 초기화
            Current_HP = FireBall_HP;//풀피
            spriteRenderer.color = FireballColor[FireBall_Lv - 1];//색 변화
        }
    }
    //HP에따른 크기
    void SizeHP()
    {
        Rate_HP = 1.0f*Current_HP / FireBall_HP;//HP비율
        transform.localScale = Vector3.Lerp(new Vector3(0.1f,0.1f,0f), new Vector3(0.3f,0.3f,0f), Rate_HP);//비율에 따른 크기
        FireBall_Y = Mathf.Lerp(-3.7f, -3.3f, Rate_HP);
        transform.position = new Vector3(0f, FireBall_Y, 0f); //불의 위치 조정
    }
    //게임 종료
    void Game_Quit()
    {
        SceneManager.LoadScene("endScene");
    }
}
