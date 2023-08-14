using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speed = 100.0f;
    Rigidbody rigid;
    public Camera cam; //메인카메라

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveKart();
    }
    
    void MoveKart()
    {
  
        //직선 이동
        if (Input.GetKey(KeyCode.UpArrow))
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            this.gameObject.transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        
        //커브 이동
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if(this.gameObject.transform.rotation.y < Mathf.Abs(15.0f))
            {
                this.gameObject.transform.Rotate(0.0f, -0.15f, 0.0f);
            }
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            if (this.gameObject.transform.rotation.y < Mathf.Abs(15.0f))
            {
                this.gameObject.transform.Rotate(0.0f, 0.15f, 0.0f);
            }
        }
        if (Input.GetKey(KeyCode.Space))
        {

        }
    }
}
