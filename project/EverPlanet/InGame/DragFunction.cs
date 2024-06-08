using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    GameObject player;
    GameObject target;
    float moveDist;//표창 이동거리
    Vector3 moveVec;

    void Awake()
    {
        monsterSpawner = GameObject.Find("MonsterSpawn").GetComponent<MonsterSpawner>();
        player = GameObject.Find("Body05");
        target = GameObject.Find("DragTarget");
    }
    private void OnEnable()
    {
        moveDist = 0f;
        moveVec = (target.transform.position - player.transform.position).normalized;
        moveVec.y = 0f;

        gameObject.transform.rotation = Quaternion.Euler(0, DotAngle(), 90);
    }
    void Update()
    {
        DragMove();
    }
    void DragMove()
    {
        gameObject.transform.position += moveVec * GameManager.Instance.PlayerDragSpeed * Time.deltaTime;
        moveDist += moveVec.sqrMagnitude;

        if (moveDist > 900f)
        {
            gameObject.SetActive(false);
        }
    }
    //표창 y축 회전 정도
    float DotAngle()
    {
        float dot = -moveVec.x;
        float cosTheta = dot / moveVec.magnitude;
        float theta = -Mathf.Acos(cosTheta) * 180 / Mathf.PI;
        return theta;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Monster")//몬스터 공격
        {
            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
            monsterSpawner.mobCount--;
        }
    }
}
