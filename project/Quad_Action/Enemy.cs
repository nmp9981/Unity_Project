using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;//체력
    public int curHealth;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponent<MeshRenderer>().material;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")//근접
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;

            Vector3 reactVec = transform.position - other.transform.position;//넉백
            StartCoroutine(OnDamage(reactVec,false));//피격 처리
        }else if(other.tag == "Bullet")//원격
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;

            Vector3 reactVec = transform.position - other.transform.position;//넉백
            Destroy(other.gameObject);//총알 삭제
            StartCoroutine(OnDamage(reactVec,false));//피격 처리
        }
    }
    //수류탄 맞음
    public void HitByGrenade(Vector3 explosionPos)
    {
        curHealth -= 100;
        Vector3 reactVec = transform.position - explosionPos;//넉백
        StartCoroutine(OnDamage(reactVec, true));//피격 처리
    }

    //피격 처리
    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)//생성
        {
            mat.color = Color.white;
        }
        else//사망
        {
            mat.color = Color.gray;
            gameObject.layer = 14;//레이어 번호 변경(더이상 물리효과 못받게)

            //넉백
            if (isGrenade)
            {
                reactVec = reactVec.normalized;
                reactVec += (Vector3.up*3);

                rigid.freezeRotation = false;//회전이 가능하게 체크를 해제
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
                rigid.AddTorque(reactVec * 15, ForceMode.Impulse);
            }
            else
            {
                reactVec = reactVec.normalized;
                reactVec += Vector3.up;
                rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 4f);
        }
    }
}
