using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DragFunction : MonoBehaviour
{
    MonsterSpawner monsterSpawner;
    GameObject player;
    GameObject target;
    float moveDist;//표창 이동거리
    Vector3 moveVec;

    [SerializeField] TextMeshProUGUI DamegeText;
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
    private void OnCollisionEnter(Collision collision)//피격
    {
        if(collision.gameObject.tag == "Monster")//몬스터 공격
        {
            DamegeText.transform.position = Camera.main.WorldToScreenPoint(collision.gameObject.transform.position + new Vector3(0, 1f, 0));
            DamegeText.text = GameManager.Instance.PlayerAttack.ToString();
            collision.gameObject.GetComponent<MonsterFunction>().monsterHP -= GameManager.Instance.PlayerAttack;
            StartCoroutine(ShowDamage());
        }
    }
    //데미지 보여주기
    IEnumerator ShowDamage()
    {
        yield return new WaitForSeconds(0.3f);
        DamegeText.text = "";
        gameObject.SetActive(false);
    }
}
