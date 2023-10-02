using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type {Melee, Range };//공격 타입(근접,원격)
    public Type type;
    public int damage;
    public float rate;//공격 속도
    public BoxCollider meleeArea;//공격 범위
    public TrailRenderer trailEffext;//공격 이펙트

    public void Use()
    {
        if (type == Type.Melee)//근접 공격
        {
            StartCoroutine("Swing");
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
}
