using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
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
        if (GameManager.Instance.PlayMode == 0)//챌린지
        {
            int ranNum = Random.Range(0, 100);
            int upCut = 90 - GameManager.Instance.StageNum;
            int middleCut = 70 - 2*GameManager.Instance.StageNum;

            if (ranNum > upCut) {//상급 
                _enemyAttack = (long)Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum) * 1024;
            }
            else if (ranNum < middleCut)//하급
            {
                _enemyAttack = (long)Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum) * Random.Range(1, 3) - Random.Range(1, GameManager.Instance.StageNum);
            }
            else//중급
            {
                _enemyAttack = (long)Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum) * 24;
            }
        }
        else//스테이지 모드
        {
            int ranNum = Random.Range(0, 100);
            if (ranNum < 90-Time.time/4)//시간에 따라 다르게
            {
                int add =4 + (int)Time.time/2;
                _enemyAttack = (long)Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum) * Random.Range(1, add) - Random.Range(1, GameManager.Instance.StageNum);
            }
            else
            {
                _enemyAttack = (long)Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum)*1024;
            }
           
        }
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
                    GameManager.Instance.PlayerScale = 1.0f 
                        + GameManager.Instance.PlayerAttack * 0.00015f / Mathf.Pow(2.0f,(float)GameManager.Instance.PlayerAttack-1);

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
