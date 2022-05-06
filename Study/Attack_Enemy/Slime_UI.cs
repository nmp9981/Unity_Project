using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime_UI : MonoBehaviour
{
    private GameObject slime;
    private Rigidbody2D rigid;
    private float horizontal_vec;
    private float speed = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        slime = GameObject.Find("Slime");
        rigid = slime.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (horizontal_vec != 0)
        {
            rigid.velocity = new Vector2(horizontal_vec, 0) * speed;
        }
    }
    public void left_move()
    {
        horizontal_vec = -1.0f;
    }
    public void right_move()
    {
        horizontal_vec = 1.0f;
    }
    public void dont_move()
    {
        horizontal_vec = 0.0f;
    }
}
