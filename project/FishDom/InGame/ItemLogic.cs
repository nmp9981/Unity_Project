using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ItemLogic : MonoBehaviour
{
    private float dir;
    void Start()
    {
        SetMoveDir();
    }

    // Update is called once per frame
    void Update()
    {
        ItemMove();
    }
    void SetMoveDir()
    {
        dir = Random.Range(0, 10);
        dir = (dir % 2 == 0) ? -1 : 1;
    }
    void ItemMove()
    {
        Vector3 ememyMoveDir = new Vector3(dir, 0, 0);
        gameObject.transform.position += ememyMoveDir * GameManager.Instance.EnemyMoveSpeed * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            StartCoroutine(PlayerNotHit());

        }
    }
    public IEnumerator PlayerNotHit()
    {
        GameManager.Instance.PlayerHit = false;
        yield return new WaitForSeconds(5.0f);
        GameManager.Instance.PlayerHit = true;
    }
}
