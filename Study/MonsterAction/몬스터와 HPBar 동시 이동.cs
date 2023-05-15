using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField]
    GameObject HPBar;
    [SerializeField]
    GameObject HPBarBack;
    
    public int idx;
    Spon spon;
    // Start is called before the first frame update
    void Awake()
    {
        idx = 0;
        spon = GameObject.Find("MonsterManager").GetComponent<Spon>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, spon.Wall[idx].transform.position,0.01f);
        HPBar.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0,0.7f,0));
        HPBarBack.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 0.7f, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            idx = (idx + 1) % 2;
            //바라보는 방향
            if (idx == 1) this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
            else if (idx == 0) this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
}
