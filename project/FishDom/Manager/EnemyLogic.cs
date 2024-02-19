using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    private float dir;
    [SerializeField] private TextMeshProUGUI _enemyAttackText;

    void Start()
    {
        SetMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyMove();
        EnemyAttackTextMove();
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
        _enemyAttackText.text = 1.ToString();
    }
}
