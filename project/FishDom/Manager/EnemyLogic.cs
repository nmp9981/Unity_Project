using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    InputManager _inputManager;
    private float dir;
    [SerializeField] private TextMeshProUGUI _enemyAttackText;
    private long _enemyAttack;

    private void Awake()
    {
        _inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }
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
        EnemyToCameraDistance();
    }
    void AttackSetting()
    {
        _enemyAttack = GameManager.Instance.StageNum*Random.Range(1, 3);
    }
    void SetMoveDir()
    {
        dir = Random.Range(0, 10);
        dir = (dir % 2 == 0) ? -1 : 1;

        if(GameManager.Instance.StageNum==1) gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        else if(GameManager.Instance.StageNum <= 3) gameObject.transform.rotation = Quaternion.Euler(0,dir*90,0);
    }
    void EnemyMove()
    {
        Vector3 ememyMoveDir = new Vector3(dir, 0, 0);
        gameObject.transform.position += ememyMoveDir * GameManager.Instance.EnemyMoveSpeed * Time.deltaTime;
    }
    void EnemyAttackTextMove()
    {
        _enemyAttackText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, gameObject.transform.localScale.y, 0));
        _enemyAttackText.text = _enemyAttack.ToString();
    }
    void EnemyToCameraDistance()
    {
        float _enemyToCameraDistanceX = Camera.main.transform.position.x - gameObject.transform.position.x;
        float _enemyToCameraDistanceY = Camera.main.transform.position.y - gameObject.transform.position.y;
        float _enemyToCameraDistance = _enemyToCameraDistanceX * _enemyToCameraDistanceX + _enemyToCameraDistanceY * _enemyToCameraDistanceY;

        //카메라로 부터 일정거리를 벗어나면 비활성화
        if (_enemyToCameraDistance > GameManager.Instance.EnemyToCameraStdDistance) gameObject.SetActive(false);
        else gameObject.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!GameManager.Instance.PlayerHit)
            {
                gameObject.SetActive(false);
            }
            else
            {
                if (_enemyAttack < GameManager.Instance.PlayerAttack)
                {
                    gameObject.SetActive(false);
                    GameManager.Instance.PlayerAttack += _enemyAttack;
                    GameManager.Instance.PlayerScale = 1.0f + GameManager.Instance.PlayerAttack * 0.0015f;

                    if (GameManager.Instance.PlayMode != 0)
                    {
                        if(GameManager.Instance.PlayMode > GameManager.Instance.PlayerMaxAttack)//최고 공격력 저장
                        {
                            GameManager.Instance.PlayerMaxAttack = GameManager.Instance.PlayMode;
                            PlayerPrefs.SetString("BestAttack", GameManager.Instance.PlayerMaxAttack.ToString());
                        }
                    }
                }
                else
                {
                    _inputManager.GameOver();
                }
            }
        }
    }
}
