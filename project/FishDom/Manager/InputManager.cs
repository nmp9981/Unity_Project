using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerAttackText;
    [SerializeField] TextMeshProUGUI _playTimeText;
    [SerializeField] private GameObject _gameOverBox;
    [SerializeField] private GameObject _gameClearBox;
    [SerializeField] private ParticleSystem _notHitEffect;

    [SerializeField] GameObject _upButton;
    [SerializeField] GameObject _downButton;
    [SerializeField] GameObject _leftButton;
    [SerializeField] GameObject _rightButton;

    ObjectFulling _objectFull;
    SoundManager _soundManager;

    public Vector3 moveDir;

    public bool upMove = false;
    public bool downMove = false;
    public bool leftMove = false;
    public bool rightMove = false;

    void Awake()
    {
        _soundManager = GameObject.Find("SoundManager").GetComponent<SoundManager>();
        _objectFull = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        _objectFull.OffObj();
        _gameClearBox.gameObject.SetActive(false);
        _gameOverBox.gameObject.SetActive(false);

        GameManager.Instance.PlayerMaxTime = 0;//경과 시간 초기화

#if UNITY_EDITOR
        _upButton.gameObject.SetActive(false);
        _downButton.gameObject.SetActive(false);
        _leftButton.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(false);
#elif UNITY_STANDALONE_WIN
        _upButton.gameObject.SetActive(false);
        _downButton.gameObject.SetActive(false);
        _leftButton.gameObject.SetActive(false);
        _rightButton.gameObject.SetActive(false);
#else
        _upButton.gameObject.SetActive(true);
        _upButton.gameObject.SetActive(true);
        _upButton.gameObject.SetActive(true);
        _upButton.gameObject.SetActive(true);
#endif

    }
    private void Start()
    {
        if (GameManager.Instance.PlayMode == 0)
        {
            GameManager.Instance.PlayerAttack = 2;
            GameManager.Instance.StageNum = 1;
        }
        else
        {
            GameManager.Instance.PlayerAttack = (long) Mathf.Pow(2.0f, (float)GameManager.Instance.StageNum)*(GameManager.Instance.StageNum/10+1);
        }
    }
    void Update()
    {
        PlayTimeTextShow();
        PlayerMove();
        PlayerAttackTextMove();
        PlayerScaleUp();
        PlayerNotHitEffect();
        PlayerTimeRecord();
        GameManager.Instance.StageUp();
    }
    void PlayTimeTextShow()
    {
        if (GameManager.Instance.PlayMode == 0)
        {
            int timer = (int)GameManager.Instance.PlayerMaxTime;
            _playTimeText.text = "Time : " + timer.ToString();
        }
    }
   public void PlayerMove()
    {
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow)) GameManager.Instance.PlayerDir = -1.0f;
        else if(Input.GetKey(KeyCode.RightArrow)) GameManager.Instance.PlayerDir = 1.0f;
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        moveDir = new Vector3(hAxis, vAxis,0);
        gameObject.transform.position += moveDir * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
#elif UNITY_STANDALONE_WIN
        if (Input.GetKey(KeyCode.LeftArrow)) GameManager.Instance.PlayerDir = -1.0f;
        else if(Input.GetKey(KeyCode.RightArrow)) GameManager.Instance.PlayerDir = 1.0f;
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        moveDir = new Vector3(hAxis, vAxis,0);
        gameObject.transform.position += moveDir * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
#endif
        gameObject.transform.localRotation = Quaternion.Euler(0, -90.0f * GameManager.Instance.PlayerDir, 0);
    }

    public void MoveUPOn()
    {
        upMove = true;
    }
    public void MoveUPOff()
    {
        upMove = false;
    }
    public void MoveDownOn()
    {
        downMove = true;
    }
    public void MoveDownOff()
    {
        downMove = false;
    }
    public void MoveLeftOn()
    {
        GameManager.Instance.PlayerDir = -1.0f;
        leftMove = true;
    }
    public void MoveLeftOff()
    {
        leftMove = false;
    }
    public void MoveRightOn()
    {
        GameManager.Instance.PlayerDir = 1.0f;
        rightMove = true;
    }
    public void MoveRightOff()
    {
        rightMove = false;
    }
   
    void PlayerAttackTextMove()
    {
        _playerAttackText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position+new Vector3(0,1.0f,0));
        _playerAttackText.text = string.Format("{0:#,0}", GameManager.Instance.PlayerAttack);
    }
    void PlayerScaleUp()
    {
        gameObject.transform.localScale = GameManager.Instance.PlayerScale*new Vector3(0.1f,0.1f,0.1f);
    }
    void PlayerNotHitEffect()
    {
        if (GameManager.Instance.PlayerHit)
        {
            if (_notHitEffect.isPlaying) _notHitEffect.Stop();
        }
        else
        {
            if (!_notHitEffect.isPlaying)
            {
                _notHitEffect.GetComponent<ParticleSystem>().startSize = gameObject.transform.localScale.y * 1.1f;
                _notHitEffect.Play();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            StartCoroutine(PlayerNotHit());
            other.gameObject.SetActive(false);
        }
    }
    public IEnumerator PlayerNotHit()
    {
        _soundManager.PlaySfx(SFXSound.Star);
        GameManager.Instance.PlayerHit = false;
        yield return new WaitForSeconds(5.0f);
        GameManager.Instance.PlayerHit = true;
    }
    void PlayerTimeRecord()
    {
        if (GameManager.Instance.PlayMode == 0)//챌린지 모드일때만 기록
        {
            if (GameManager.Instance.PlayerMaxTime > PlayerPrefs.GetFloat("BestTime"))
            {
                PlayerPrefs.SetFloat("BestTime", GameManager.Instance.PlayerMaxTime);
            }
        }
    }
    //게임 클리어
    public void GameClear()
    {
        _soundManager.PlaySfx(SFXSound.Clear);
        if (GameManager.Instance.PlayMode != 0)
        {
            if (GameManager.Instance.StageNum >= PlayerPrefs.GetFloat("BestClear"))
            {
                GameManager.Instance.PlayerMaxClearStage += 1;
                PlayerPrefs.SetFloat("BestClear", GameManager.Instance.PlayerMaxClearStage);
            }
        }
        _gameClearBox.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
    //게임 오버
    public void GameOver()
    {
        _soundManager.PlaySfx(SFXSound.Die);
        _gameOverBox.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void GoMainScene()
    {
        GameInit();
        SceneManager.LoadScene("MainMenu");
    }
    public void GameInit()
    {
        GameManager.Instance.PlayerHit = true;
        GameManager.Instance.StageNum = 1;
        GameManager.Instance.PlayInitTime = -7.0f;
    }
}
