using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public int Attack;
    public float speed;
    public GameObject target;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            gameObject.SetActive(false);
            MonsterHit(collision.gameObject);
        }
    }
    private void Update()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, target.transform.position, Time.deltaTime * speed);
    }
    /// <summary>
    /// 기능 : 타겟 세팅
    /// </summary>
    public void TargetSetting(GameObject targ)
    {
        target = targ;
        transform.LookAt(target.transform.position);
    }
    void MoveThrowObject()
    {
        Vector3.MoveTowards(gameObject.transform.position, target.transform.position, Time.deltaTime * speed);
    }
    void MonsterHit(GameObject gm)
    {
        Destroy(gm);
    }
    
}
