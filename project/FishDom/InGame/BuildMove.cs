using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildMove : MonoBehaviour
{
    InputManager _inputManager;
    void Awake()
    {
        _inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }

    void Update()
    {
#if UNITY_EDITOR
#else
        if (Input.GetMouseButton(0))//마우스 우클릭 or 빌드상에서 버튼 눌렀을 때
        {
            if (_inputManager.upMove) _inputManager.moveDir = new Vector3(0, 1, 0);
            if (_inputManager.downMove) _inputManager.moveDir = new Vector3(0, -1, 0);
            if (_inputManager.leftMove) _inputManager.moveDir = new Vector3(-1, 0, 0);
            if (_inputManager.rightMove) _inputManager.moveDir = new Vector3(1, 0, 0);

            gameObject.transform.position += _inputManager.moveDir * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
        }
#endif
    }
}
