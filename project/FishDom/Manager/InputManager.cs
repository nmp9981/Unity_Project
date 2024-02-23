using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerAttackText;
    [SerializeField] private GameObject _gameOverBox;

    void Awake()
    {
        _gameOverBox.gameObject.SetActive(false);
    }
    private void Start()
    {
        
    }
    void Update()
    {
        PlayerMove();
        PlayerAttackTextMove();
        PlayerScaleUp();
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
    //게임 오버
    public void GameOver()
    {
        _gameOverBox.gameObject.SetActive(true);
        Time.timeScale = 0.0f;
    }
}
