using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BuildMove : MonoBehaviour
{
    InputManager _inputManager;
    // Start is called before the first frame update
    void Awake()
    {
        _inputManager = GameObject.Find("Player").GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (_inputManager.upMove) _inputManager.moveDir = new Vector3(0, 1, 0);
            if (_inputManager.downMove) _inputManager.moveDir = new Vector3(0, -1, 0);
            if (_inputManager.leftMove) _inputManager.moveDir = new Vector3(-1, 0, 0);
            if (_inputManager.rightMove) _inputManager.moveDir = new Vector3(1, 0, 0);

            gameObject.transform.position += _inputManager.moveDir * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
        }
    }
}
