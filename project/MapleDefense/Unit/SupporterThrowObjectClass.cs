using UnityEngine;

public class SupporterThrowObjectClass : MonoBehaviour
{
    /// <summary>
    /// 몬스터 공격력
    /// </summary>
    public int supporterBasicAttack;

    [SerializeField]
    float moveSpeed;

    private void Update()
    {
        MoveToCastle();
    }
    /// <summary>
    /// 직선 이동
    /// </summary>
    void MoveToCastle()
    {
        transform.position += Vector3.right*moveSpeed*Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("Enemy"))
        {
            //피격 효과음
            SoundManager._sound.PlaySfx((int)SFXSound.CastleHit);
            EnemyUnit enemyObj = collision.gameObject.GetComponent<EnemyUnit>();
            DecreaseCastleHP(supporterBasicAttack, enemyObj);
        }
    }
    /// <summary>
    /// HP 감소
    /// </summary>
    /// <param name="throwAttack">투사체 공격력</param>
    void DecreaseCastleHP(int throwAttack, EnemyUnit enemy)
    {
        enemy.HP -= throwAttack;
        Destroy(this.gameObject);
    }
}
