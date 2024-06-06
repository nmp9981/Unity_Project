using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    GameObject player;
    GameObject target;
    float moveDist;//표창 이동거리
    Vector3 moveVec;

    void Awake()
    {
        player = GameObject.Find("Body05");
        target = GameObject.Find("DragTarget");
    }
    private void OnEnable()
    {
        moveDist = 0f;
        moveVec = (target.transform.position - player.transform.position).normalized;
        moveVec.y = 0f;
    }
    // Update is called once per frame
    void Update()
    {
        DragMove();
    }
    void DragMove()
    {
        gameObject.transform.position += moveVec * GameManager.Instance.PlayerAttackSpeed * Time.deltaTime;
        moveDist += moveVec.sqrMagnitude;

        if (moveDist > 1000f)
        {
            gameObject.SetActive(false);
        }
    }
}
