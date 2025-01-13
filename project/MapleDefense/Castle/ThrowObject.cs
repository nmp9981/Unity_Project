using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public int Attack;
    public float speed;
    public GameObject target;

    //어느 무기에서 발사했는가?
    public CastleAttack fromWeapon;

    Vector3 dir;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        gameObject.transform.position += dir*Time.deltaTime*speed;
    }
    /// <summary>
    /// 기능 : 타겟 세팅
    /// </summary>
    public void TargetSetting(GameObject targ)
    {
        target = targ;
        transform.LookAt(target.transform.position);
        //y축 값을 0으로 (y값만 변경)
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.rotation.x,0,
         gameObject.transform.rotation.z);
        dir = (target.transform.position - gameObject.transform.position).normalized;

    }
}
