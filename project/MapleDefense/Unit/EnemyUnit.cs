using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.UI;

//애니메이터 변수 관리용
enum AnimatorVar
{
    isMoving,
    isHit,
    isDie,
    isAttack
}

public class EnemyInfo
{
    public SpriteRenderer spriteRenderer;
    public int HP;
    public int Exp;
    public int Attack;

    public EnemyInfo(SpriteRenderer spriteRenderer, int hP, int exp, int attack)
    {
        this.spriteRenderer = spriteRenderer;
        HP = hP;
        Exp = exp;
        Attack = attack;
    }
}
public class EnemyUnit : MonoBehaviour
{
    [SerializeField]
    Image hpBarBack;
    [SerializeField]
    Image hpBar;

    public int FullHP;
    public int HP;
    public uint Exp;
    public int Attack;
    public ulong Meso;

    Animator anim;

    float moveSpeed = 2f;
   
    private void Awake()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetBool("isDie", false);
        anim.SetBool("isHit", false);
    }
    private void OnEnable()
    {
        //EnemyInfo enemy = new EnemyInfo(dp,10,10,10);
        HP = FullHP;
        hpBar.fillAmount = 1f;
    }
    private void Update()
    {
        MoveEnemy();
        HPBarMove();
    }
    /// <summary>
    /// 기능 : 적 이동
    /// 성 넘어가면 오브젝트 비활성화
    /// </summary>
    void MoveEnemy()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        anim.Play("Move");
        //공격 대상에서 벗어남
        if (transform.position.x < -2f)
        {
            GameManager.Instance.ActiveUnitList.Remove(gameObject);
        }

        //비활성화
        if (transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 기능 : HPBar 이동
    /// </summary>
    void HPBarMove()
    {
        hpBarBack.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 0.85f, 0));
        hpBar.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 0.85f, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            moveSpeed = 0;
        }
        if (collision.tag.Contains("Bullet"))
        {
            //HP감소
            HP -= collision.gameObject.GetComponent<ThrowObject>().Attack;
            float hpRate = (float)HP / FullHP;
            hpBar.fillAmount = hpRate;

            //사망처리
            if (HP <= 0)
            {
                GameManager.Instance.CurrentMeso += Meso;
                GameManager.Instance.CurrentExp += Exp;
                GameManager.Instance.ActiveUnitList.Remove(gameObject);
                gameObject.SetActive(false);
            }
        }
    }

}
