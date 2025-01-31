using System.Collections;
using UnityEngine;

public class EnemyThrowObjectClass : MonoBehaviour
{
    /// <summary>
    /// 몬스터 공격력
    /// </summary>
    public int monsterBasicAttack;

    [SerializeField]
    float moveSpeed;

    Transform targetCastle;
    CastleManager castleManager;
    private void Awake()
    {
        GameObject castleObj = GameObject.Find("Castle");
        castleManager = castleObj.GetComponent<CastleManager>();
        targetCastle = castleObj.transform;
    }

    private void Update()
    {
        MoveToCastle();
    }
    void MoveToCastle()
    {
        transform.position = Vector3.MoveTowards(transform.position, (targetCastle.position + 2*Vector3.down), Time.deltaTime * moveSpeed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Contains("CastleEnter"))
        {
            //피격 효과음
            SoundManager._sound.PlaySfx((int)SFXSound.CastleHit);
            DecreaseCastleHP(monsterBasicAttack);
        }
    }
    /// <summary>
    /// HP 감소
    /// </summary>
    /// <param name="monsterAttack">몬스터 공격력</param>
    void DecreaseCastleHP(int monsterAttack)
    {
        GameManager.Instance.CurrentCastleHP -= (monsterAttack * 3 / 2);
        castleManager.ShowCastleHP();
        Destroy(this.gameObject);
    }
}
