using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject meshObj;//정상 상태
    public GameObject effectObj;//폭발
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());//시간차 로직 => 코루틴
    }
    //폭발
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);//3초뒤 폭발
        rigid.velocity = Vector3.zero;//속도 0
        rigid.angularVelocity = Vector3.zero;//회전 속도 0
        meshObj.SetActive(false);
        effectObj.SetActive(true);

        //부피가 있는 raycast(범위내에 있는적 모두를 담아야함 -> all)
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f,LayerMask.GetMask("Enemy"));
        foreach(RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<Enemy>().HitByGrenade(transform.position);//수류탄 맞음
        }
        Destroy(gameObject, 5f);
    }
}
