using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int dmg;
    public bool isRotate;//스스로 회전 하는가?

    private void Update()
    {
        if (isRotate) transform.Rotate(Vector3.up*30);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "WallBullet")
        {
            this.gameObject.SetActive(false);
            //Destroy(this.gameObject);
        }
    }
}
