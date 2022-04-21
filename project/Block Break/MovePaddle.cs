using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePaddle : MonoBehaviour
{
    [SerializeField] float BarSpeed = 0.1f;//조작 속도
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //패들 이동
        float MoveAmount = BarSpeed * (1.2f);
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(-MoveAmount,0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(MoveAmount,0, 0);
        }
    }
}
