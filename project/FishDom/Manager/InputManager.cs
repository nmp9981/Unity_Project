using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _playerAttackText;
    void Awake()
    {
        
    }
    private void Start()
    {
        
    }
    void Update()
    {
        PlayerMove();
        PlayerAttackTextMove();
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
        _playerAttackText.text = "0000";
    }
}
