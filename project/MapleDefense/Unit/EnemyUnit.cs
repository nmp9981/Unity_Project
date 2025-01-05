using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

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
    public int HP;
    public int Exp;
    public int Attack;

    Animator anim;

    float moveSpeed = 2f;
    private void OnEnable()
    {
        //EnemyInfo enemy = new EnemyInfo(dp,10,10,10);
        

    }
    private void Update()
    {
        MoveEnemy();
    }
    /// <summary>
    /// 기능 : 적 이동
    /// 성 넘어가면 오브젝트 비활성화
    /// </summary>
    void MoveEnemy()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        
        //비활성화
        if(transform.position.x < -15f)
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            moveSpeed = 0;
        }
    }
}
