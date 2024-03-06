using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerAttackText;
    [SerializeField] private GameObject _gameOverBox;
    [SerializeField] private GameObject _gameClearBox;
    [SerializeField] private ParticleSystem _notHitEffect;

    ObjectFulling _objectFull;
    void Awake()
    {
        _objectFull = GameObject.Find("ObjectManager").GetComponent<ObjectFulling>();
        _objectFull.OffObj();
        _gameClearBox.gameObject.SetActive(false);
        _gameOverBox.gameObject.SetActive(false);
    }
    private void Start()
    {
        if (GameManager.Instance.PlayMode == 0)
        {
            GameManager.Instance.PlayerAttack = 2;
            GameManager.Instance.StageNum = 1;
        }
    }
    void Update()
    {
        PlayerMove();
        PlayerAttackTextMove();
        PlayerScaleUp();
        PlayerNotHitEffect();
    }
   public void PlayerMove()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) GameManager.Instance.PlayerDir = -1.0f;
        else if(Input.GetKey(KeyCode.RightArrow)) GameManager.Instance.PlayerDir = 1.0f;
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(hAxis, vAxis,0);
        
        gameObject.transform.position += moveDir *GameManager.Instance.PlayerMoveSpeed* Time.deltaTime;
        gameObject.transform.localRotation = Quaternion.Euler(0, -90.0f * GameManager.Instance.PlayerDir, 0);
    }
    void PlayerAttackTextMove()
    {
        _playerAttackText.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position+new Vector3(0,1f,0));
        _playerAttackText.text = GameManager.Instance.PlayerAttack.ToString();
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
        GameManager.Instance.PlayerHit = false;
        yield return new WaitForSeconds(5.0f);
        GameManager.Instance.PlayerHit = true;
    }
    //게임 클리어
    public void GameClear()
    {
        _gameClearBox.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
    //게임 오버
    public void GameOver()
    {
        _gameOverBox.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
    public void GoMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
