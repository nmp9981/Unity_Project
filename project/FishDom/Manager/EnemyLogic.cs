using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyLogic : MonoBehaviour
{
    SoundManager _soundManager;
    InputManager _inputManager;
    private float dir;
    [SerializeField] private TextMeshProUGUI _enemyAttackText;
    private long _enemyAttack;

    private void Awake()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
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
        _enemyAttack = GameManager.Instance.StageNum* GameManager.Instance.StageNum * Random.Range(1, 4);
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
        _enemyAttackText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0,1.0f, 0));
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
                GameManager.Instance.RestCount -= 1;
                if (GameManager.Instance.RestCount == 0)
                {
                    _inputManager.GameClear();
                }
            }
            else
            {
                if (_enemyAttack < GameManager.Instance.PlayerAttack)
                {
                    _soundManager.PlaySfx(SFXSound.Eat);
                    gameObject.SetActive(false);
                    GameManager.Instance.PlayerAttack += _enemyAttack;
                    GameManager.Instance.PlayerScale = 1.0f + GameManager.Instance.PlayerAttack * 0.0015f;

                    if (GameManager.Instance.PlayMode == 0)//챌린지 모드
                    {
                        if(GameManager.Instance.PlayerAttack > GameManager.Instance.PlayerMaxAttack)//최고 공격력 저장
                        {
                            GameManager.Instance.PlayerMaxAttack = GameManager.Instance.PlayerAttack;
                            PlayerPrefs.SetString("BestAttack", GameManager.Instance.PlayerMaxAttack.ToString());
                        }
                    }
                    else//스테이지 모드
                    {
                        GameManager.Instance.RestCount -= 1;
                        if (GameManager.Instance.RestCount == 0)
                        {
                            _inputManager.GameClear();
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
