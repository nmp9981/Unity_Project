using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    [SerializeField] GameObject player;
    float moveDist;//표창 이동거리

    void Start()
    {
        moveDist = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        DragMove();
    }
    void DragMove()
    {
        Vector3 moveVec = (Camera.main.transform.position - player.transform.position).normalized;
        gameObject.transform.position += moveVec * GameManager.Instance.PlayerAttackSpeed * Time.deltaTime;
        moveDist += moveVec.sqrMagnitude;

        if (moveDist > 100f) gameObject.SetActive(false);
    }
}
