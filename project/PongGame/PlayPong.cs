using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPong : MonoBehaviour
{
    [SerializeField] float BarSpeed = 0.05f;//조작 속도

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //이동
        float MoveAmount = BarSpeed*(1.1f);
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, MoveAmount, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, -MoveAmount, 0);
        }
    }
}
