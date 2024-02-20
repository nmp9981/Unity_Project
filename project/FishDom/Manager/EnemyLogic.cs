using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    private float dir;
    [SerializeField] private TextMeshProUGUI _enemyAttackText;
    private long _enemyAttack;

    void Start()
    {
        AttackSetting();
        SetMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMove();
        EnemyAttackTextMove();
    }
    void AttackSetting()
    {
        _enemyAttack = GameManager.Instance.StageNum*Random.Range(1, 3);
    }
    void SetMoveDir()
    {
        dir = Random.Range(0, 10);
        dir = (dir % 2 == 0) ? -1 : 1;
        gameObject.transform.rotation = Quaternion.Euler(90,0,0);
    }
    void EnemyMove()
    {
        Vector3 ememyMoveDir = new Vector3(dir, 0, 0);
        gameObject.transform.position += ememyMoveDir * GameManager.Instance.EnemyMoveSpeed * Time.deltaTime;
    }
    void EnemyAttackTextMove()
    {
        _enemyAttackText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 1f, 0));
        _enemyAttackText.text = _enemyAttack.ToString();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (_enemyAttack < GameManager.Instance.PlayerAttack)
            {
                gameObject.SetActive(false);
                GameManager.Instance.PlayerAttack += _enemyAttack;
            }
            else
            {
                //게임 오버
            }
        }
    }
}
