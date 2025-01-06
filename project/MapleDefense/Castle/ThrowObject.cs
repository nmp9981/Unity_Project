using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

public class ThrowObject : MonoBehaviour
{
    public int Attack;
    public float speed;

    public GameObject target;
    
    private void OnEnable()
    {
        if (target != null)
        {
            transform.LookAt(target.transform.position);
        }
    }
    void Update()
    {
        if (target != null)
        {
            Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * speed);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Enemy"))
        {
            gameObject.SetActive(false);
            MonsterHit(collision.gameObject);
        }
    }
    void MonsterHit(GameObject gm)
    {
        Destroy(gm);
    }
    
}
