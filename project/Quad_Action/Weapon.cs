using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {Melee, Range };//공격 타입(근접,원격)
    public Type type;
    public int damage;
    public float rate;//공격 속도
    public int maxAmmo;//최대 총알
    public int curAmmo;//현재 총알 개수
    
    public BoxCollider meleeArea;//공격 범위
    public TrailRenderer trailEffext;//공격 이펙트
    public Transform bulletPos;//총알이 나오는 위치
    public GameObject bullet;//총알 프리팹
    public Transform bulletCasePos;//탄피가 나오는 위치
    public GameObject bulletCase;//탄피 프리팹 
    public void Use()
    {
        if (type == Type.Melee)//근접 공격
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }else if (type == Type.Range && curAmmo>0)//원격 공격(총알 남아 있어야함)
        {
            curAmmo--;
            StartCoroutine("Shot");
        }
    }
    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);//결과 반환(대기->시간차 로직)
        meleeArea.enabled = true;//범위 활성화
        trailEffext.enabled = true;//이펙트 활성화

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailEffext.enabled = false;
        yield break;//코루틴 탈출
    }
    IEnumerator Shot()
    {
        //총알 발사
        GameObject intantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        Rigidbody bulletRigid = intantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPos.forward * 50;//앞 방향으로 50의 속도

        //탄피 배출
        GameObject intantCaseBullet = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody caseRigid = intantCaseBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        caseRigid.AddForce(caseVec, ForceMode.Impulse);
        caseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);//회전 힘(축 기준)

        yield return null;
    }
}
